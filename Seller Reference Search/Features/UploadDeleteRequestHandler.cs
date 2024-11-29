using MediatR;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Models.Commands;

namespace PrivilegePro.Features
{
    public class UploadDeleteRequestHandler : IRequestHandler<DeleteUploadsCommand, bool>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UploadDeleteRequestHandler> _logger;
        private readonly static string deleteFileUploadSql = @"DELETE FROM public.""FileUploads"" WHERE ""Id"" = {0};";

        public UploadDeleteRequestHandler(AppDbContext dbContext,
            ILogger<UploadDeleteRequestHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUploadsCommand request, CancellationToken cancellationToken)
        {
            if(request.Ids.Length == 1 && request.Ids[0] == -1)
            {
               //Todo: Delete all uploads
            }
            else
            {

                try
                {
                    var uploadRecord = await _dbContext.FileUploads.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == request.Ids[0]);

                    if (uploadRecord == null)
                        return false;

                    using(var conn = _dbContext.Database.GetDbConnection())
                    {
                        await conn.OpenAsync();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format(deleteFileUploadSql, request.Ids[0]);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    //Delete the file as well
                    File.Delete(uploadRecord.UploadPath);
                }
                catch (Exception ex) {
                    _logger.LogError("Error deleting an upload {@Exception}", ex);
                    throw;
                }
            }

            return true;
        }
    }
}
