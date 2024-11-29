using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Extensions;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Infrastructure.Data.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Seller_Reference_Search.Services
{
    public class ExcelService
    {
        private readonly int _batchSize = 500;
        public async Task ProcessExcelFileAsync(FileUpload fileUpload, AppDbContext dbContext, ILogger logger, CancellationToken cancellationToken)
        {
            var timeAtStart = DateTime.Now;
            try
            {
                // Register the code page provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                int rowCount = 0;
                string checksum = null;

                using (var stream = new FileStream(fileUpload.UploadPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Skip the header row
                        if (reader.Read())
                        {
                            var batch = new List<SaleTypeDto>();

                            while (reader.Read())
                            {
                                // Check for cancellation
                                cancellationToken.ThrowIfCancellationRequested();

                                rowCount++;
                                // Read the current row into an array
                                var rowValues = new object[reader.FieldCount];
                                reader.GetValues(rowValues);
                                if (rowValues == null)
                                    continue;

                                var sale = TransformRow(rowValues, fileUpload, rowCount, logger);
                                if (sale != null)
                                    batch.Add(sale);


                                // Process batch
                                if (rowCount % _batchSize == 0)
                                {
                                    //Process batch logic
                                    if (batch.Count > 0)
                                    {
                                        await ProcessBatch(batch, dbContext, logger);
                                        batch.Clear();
                                    }
                                }
                            }

                            // Process any remaining rows not forming a full batch
                            if (batch.Count > 0)
                            {
                                await ProcessBatch(batch, dbContext, logger);
                            }
                        }
                        //generate checksum
                        try
                        {
                            using (var sha256 = SHA256.Create())
                            {
                                byte[] hashBytes = sha256.ComputeHash(stream);

                                // Convert the byte array to a hexadecimal string
                                StringBuilder hashStringBuilder = new StringBuilder();
                                foreach (byte b in hashBytes)
                                {
                                    hashStringBuilder.Append(b.ToString("x2"));
                                }

                                checksum = hashStringBuilder.ToString();
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error generating checksum of the file: {fileUpload.UploadPath}");
                        }
                    }

                }

                //Update status
                try
                {
                    //Update row counts
                    var rowsInserted = await dbContext.Sales.CountAsync(s => s.FileUploadId == fileUpload.Id &&
                    s.CreatedAt == s.LastModifiedAt);

                    var rowsUpdated = await dbContext.Sales.CountAsync(s => s.FileUploadId == fileUpload.Id &&
                        s.CreatedAt != s.LastModifiedAt);

                    var uploader = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.Id == fileUpload.UploadedByUserId);


                    var fileUploadEntity = await dbContext.FileUploads.FirstOrDefaultAsync(f => f.Id == fileUpload.Id);
                    if (fileUploadEntity != null)
                    {
                        var processingTime = (DateTime.Now - timeAtStart);
                        if (fileUploadEntity.UploadDuration == null)
                            fileUploadEntity.UploadDuration = TimeSpan.Zero;

                        fileUploadEntity.UploadDuration += processingTime;

                        fileUploadEntity.Checksum = checksum;
                        fileUploadEntity.Status = "Success";
                        fileUploadEntity.RowsFound = rowCount;
                        fileUploadEntity.RowsInserted = rowsInserted;
                        fileUploadEntity.RowsUpdated = rowsUpdated;
                        fileUploadEntity.Description = $"File size: {fileUploadEntity.FileSize} MB, rows found: {rowCount}, rows inserted: {rowsInserted}" +
                            $", rows updated: {rowsUpdated}. Uploaded by: {uploader?.FirstName} {uploader?.LastName} - {uploader?.Email}. Upload duration:{Math.Round(fileUploadEntity.UploadDuration.Value.TotalSeconds, 2)} seconds.";
                        fileUploadEntity.LastModified = DateTime.Now.ToUniversalTime();

                        dbContext.FileUploads.Update(fileUploadEntity);
                        await dbContext.SaveChangesAsync();
                    };

                    

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error updating file upload status, Id: {fileUpload.Id}");
                    
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error reading and loading the Excel file: {fileUpload.UploadPath}");
                try
                {
                    var fileUploadEntity = await dbContext.FileUploads.FirstOrDefaultAsync(f => f.Id == fileUpload.Id);
                    var uploader = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.Id == fileUpload.UploadedByUserId);
                    if (fileUploadEntity != null)
                    {
                        fileUploadEntity.Status = "Failure";
                        fileUploadEntity.Description = ex.Message.Split(".")[0];
                        fileUploadEntity.Description = $"Uploaded by: {uploader?.FirstName} {uploader.LastName} - {uploader?.Email}.";

                        dbContext.FileUploads.Update(fileUploadEntity);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch
                {
                    logger.LogError(ex, $"Error updating failure status of file upload, Id: {fileUpload.Id}");
                }
            }

        }

        private SaleTypeDto TransformRow(object[] row, FileUpload fileUploadInfo, int rowIndex, ILogger logger)
        {
            var sale = new SaleTypeDto
            {
                FileUploadId = fileUploadInfo.Id,
                ClosingDate = DateTime.Now.ToUniversalTime()
            };

            try
            {
                /** Mandatory fields **/

                sale.Reference = Convert.ToString(row[12 - 1]).Trim().ForgivingSubstring(0, 100); // column L
                sale.ParcelNumber = Convert.ToString(row[22 - 1]).Trim().ForgivingSubstring(0, 50); // V
                sale.LotAcreage = Convert.ToDouble(row[20 - 1]); // T
                sale.OfferPrice = decimal.Parse(Regex.Replace(Convert.ToString(row[13 - 1]), "[^\\d.]", "")); //M
                sale.County = Convert.ToString(row[40 - 1]).Trim().ForgivingSubstring(0, 255);
                sale.State = Convert.ToString(row[37 - 1]).Trim().ForgivingSubstring(0, 2).ToUpper();

                /** Mandatory fields end **/

                try
                {
                    string value = Convert.ToString(row[23 - 1])?.Trim() ?? string.Empty; // W
                    sale.OwnerName = value.Length > 500 ? value.ForgivingSubstring(0, 500) : value;
                }
                catch { }

                if (decimal.TryParse(Regex.Replace(row[15 - 1]?.ToString() ?? string.Empty, "[^\\d.]", ""), out decimal offerPPA)) // O
                {
                    sale.OfferPPA = offerPPA;
                }

                if (decimal.TryParse(Regex.Replace(row[17 - 1]?.ToString() ?? string.Empty, "[^\\d.]", ""), out decimal realPPA)) // Q
                {
                    sale.RealPPA = realPPA;
                }

                if (decimal.TryParse(Regex.Replace(row[16 - 1]?.ToString() ?? string.Empty, "[^\\d.]", ""), out decimal ppaCalc)) // P
                {
                    sale.PPACalc = ppaCalc;
                }

                if (decimal.TryParse(Regex.Replace(row[19 - 1]?.ToString() ?? string.Empty, "[^\\d.]", ""), out decimal profit)) // S
                {
                    sale.Profit = profit;
                }

                if (decimal.TryParse(Regex.Replace(row[18 - 1]?.ToString() ?? string.Empty, "[^\\d.]", ""), out decimal retailValue)) // R
                {
                    sale.RetailValue = retailValue;
                }

                try
                {
                    string value = Convert.ToString(row[38 - 1])?.Trim() ?? string.Empty; // AL
                    sale.ZipCode = value.Length > 10 ? value.ForgivingSubstring(0, 10) : value;
                }
                catch { }

                try
                {
                    string value = Convert.ToString(row[14 - 1])?.Trim() ?? string.Empty; // N
                    bool isParsed = DateTime.TryParse(value, out DateTime parsedDate);
                    if (isParsed)
                        sale.ClosingDate = parsedDate;
                }
                catch { }


            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error parsing row number {rowIndex} of file: {fileUploadInfo.UploadPath}");
                return null;
            }

            return sale;
        }

        private async Task ProcessBatch(List<SaleTypeDto> sales, AppDbContext dbContext, ILogger logger)
        {
            try
            {
                //var json = JsonConvert.SerializeObject(sales);
                var json = System.Text.Json.JsonSerializer.Serialize(sales);
                await dbContext.Database.ExecuteSqlRawAsync("CALL load_sales({0}::jsonb)", new object[] { json });

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error loading records into the database: {ex.Message}");
            }
        }
    }

}
