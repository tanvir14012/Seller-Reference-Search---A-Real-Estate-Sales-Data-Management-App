using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data.Models;
using System.Text;

namespace Seller_Reference_Search.Infrastructure.Data
{
    public class AppDbContextSeed
    {

        private static readonly string loadSalesSprocSql =
        @"
        CREATE OR REPLACE PROCEDURE public.load_sales(
	        IN sales_data jsonb)
        LANGUAGE 'plpgsql'
        AS $BODY$
        BEGIN
	
	        MERGE INTO ""Sales"" AS target
	        USING (
                SELECT 
                    (j->>'Reference')::VARCHAR(100) COLLATE pg_catalog.""default"" AS ""Reference"",
                    (j->>'OwnerName')::VARCHAR(500) COLLATE pg_catalog.""default"" AS ""OwnerName"",
                    (j->>'ParcelNumber')::VARCHAR(50) COLLATE pg_catalog.""default"" AS ""ParcelNumber"",
                    (j->>'LotAcreage')::DOUBLE PRECISION AS ""LotAcreage"",
                    (j->>'OfferPrice')::NUMERIC(18,2) AS ""OfferPrice"",
                    (j->>'OfferPPA')::NUMERIC(18,2) AS ""OfferPPA"",
                    (j->>'RealPPA')::NUMERIC(18,2) AS ""RealPPA"",
                    (j->>'PPACalc')::NUMERIC(18,2) AS ""PPACalc"",
                    (j->>'Profit')::NUMERIC(18,2) AS ""Profit"",
                    (j->>'RetailValue')::NUMERIC(18,2) AS ""RetailValue"",
                    (j->>'County')::VARCHAR(255) COLLATE pg_catalog.""default"" AS ""County"",
                    (j->>'State')::VARCHAR(2) COLLATE pg_catalog.""default"" AS ""State"",
                    (j->>'ZipCode')::VARCHAR(10) COLLATE pg_catalog.""default"" AS ""ZipCode"",
                    (j->>'ClosingDate')::TIMESTAMP WITHOUT TIME ZONE AS ""ClosingDate"",
                    (j->>'FileUploadId')::INTEGER AS ""FileUploadId""
                FROM jsonb_array_elements(sales_data) AS j
            ) AS source
	        ON target.""Reference"" = source.""Reference""
	        WHEN MATCHED AND (
		        target.""OwnerName"" IS DISTINCT FROM source.""OwnerName"" OR
		        target.""ParcelNumber"" IS DISTINCT FROM source.""ParcelNumber"" OR
		        target.""LotAcreage"" IS DISTINCT FROM source.""LotAcreage"" OR
		        target.""OfferPrice"" IS DISTINCT FROM source.""OfferPrice"" OR
		        target.""OfferPPA"" IS DISTINCT FROM source.""OfferPPA"" OR
		        target.""RealPPA"" IS DISTINCT FROM source.""RealPPA"" OR
		        target.""PPACalc"" IS DISTINCT FROM source.""PPACalc"" OR
		        target.""Profit"" IS DISTINCT FROM source.""Profit"" OR
		        target.""RetailValue"" IS DISTINCT FROM source.""RetailValue"" OR
		        target.""County"" IS DISTINCT FROM source.""County"" OR
		        target.""State"" IS DISTINCT FROM source.""State"" OR
		        target.""ZipCode"" IS DISTINCT FROM source.""ZipCode"" OR
		        target.""ClosingDate"" IS DISTINCT FROM source.""ClosingDate""
	        ) THEN
		        UPDATE SET
			        ""OwnerName"" = source.""OwnerName"",
			        ""ParcelNumber"" = source.""ParcelNumber"",
			        ""LotAcreage"" = source.""LotAcreage"",
			        ""OfferPrice"" = source.""OfferPrice"",
			        ""OfferPPA"" = source.""OfferPPA"",
			        ""RealPPA"" = source.""RealPPA"",
			        ""PPACalc"" = source.""PPACalc"",
			        ""Profit"" = source.""Profit"",
			        ""RetailValue"" = source.""RetailValue"",
			        ""County"" = source.""County"",
			        ""State"" = source.""State"",
			        ""ZipCode"" = source.""ZipCode"",
			        ""ClosingDate"" = source.""ClosingDate"",
			        ""FileUploadId"" = source.""FileUploadId"",
			        ""LastModifiedAt"" = NOW()
	        WHEN NOT MATCHED THEN
		        INSERT (
			        ""Reference"",
			        ""OwnerName"",
			        ""ParcelNumber"",
			        ""LotAcreage"",
			        ""OfferPrice"",
			        ""OfferPPA"",
			        ""RealPPA"",
			        ""PPACalc"",
			        ""Profit"",
			        ""RetailValue"",
			        ""County"",
			        ""State"",
			        ""ZipCode"",
			        ""ClosingDate"",
			        ""FileUploadId"",
			        ""CreatedAt"",
			        ""LastModifiedAt""
		        )
		        VALUES (
			        source.""Reference"",
			        source.""OwnerName"",
			        source.""ParcelNumber"",
			        source.""LotAcreage"",
			        source.""OfferPrice"",
			        source.""OfferPPA"",
			        source.""RealPPA"",
			        source.""PPACalc"",
			        source.""Profit"",
			        source.""RetailValue"",
			        source.""County"",
			        source.""State"",
			        source.""ZipCode"",
			        source.""ClosingDate"",
			        source.""FileUploadId"",
			        NOW(),
			        NOW()
		        );
        END
        
        $BODY$;
        ";

        public static readonly string getSearch =
            @"CREATE OR REPLACE FUNCTION public.prepare_search () 
            RETURNS JSONB
            LANGUAGE 'plpgsql' 
            AS $BODY$
            DECLARE
                max_acres numeric(18, 2) := 0;
                offer_price_max numeric(18, 2) := 0;
                closing_date_max date := '9999-12-31';
                states JSONB;
                max_values JSONB;
            BEGIN
                SELECT MAX(""LotAcreage"") INTO max_acres FROM public.""Sales"";
                SELECT MAX(""OfferPrice"") INTO offer_price_max FROM public.""Sales"";
                SELECT MAX(""ClosingDate"") INTO closing_date_max FROM public.""Sales"";

                max_values := jsonb_build_object(
                    'max_acres', max_acres,
                    'offer_price_max', offer_price_max,
                    'closing_date_max', closing_date_max
                );

                WITH get_states AS (
                    SELECT DISTINCT ""State"" AS state FROM public.""Sales""
                    ORDER BY ""State""
                ) 
                SELECT jsonb_build_object(
                    'states', jsonb_agg(
                    state
                    )) 
                INTO states
                FROM get_states;

                RETURN jsonb_concat(max_values, states);
            END;
            $BODY$;

            ";

        public static readonly string searchSql = @"
            CREATE OR REPLACE FUNCTION public.search_sales(
	            query_text text,
	            cnt_query_text text,
	            draw integer)
                RETURNS jsonb
                LANGUAGE 'plpgsql'
            AS $BODY$
            DECLARE
                ""recordsTotal"" numeric(10,0) := 0;
                ""recordsFiltered"" numeric(10,0) := 0;
                ""data"" jsonb;
            BEGIN
                -- Calculate the total number of records in the Sales table
                SELECT COUNT(*) INTO ""recordsTotal"" FROM public.""Sales"";

	            -- Calculate the filtered records count
                EXECUTE cnt_query_text INTO ""recordsFiltered"";

                -- Execute the dynamic query, aggregate the results into JSONB, and count the filtered rows
	            EXECUTE format(
	                'WITH results_cte AS (
	                    %s
	                )
	                SELECT jsonb_agg(row_to_json(t))
	                FROM results_cte t',
	                query_text
	            )
	            INTO ""data"";    

                -- Construct and return the JSONB result
                RETURN jsonb_build_object(
                    'draw', draw,
                    'recordsTotal', ""recordsTotal"",
                    'recordsFiltered', ""recordsFiltered"",
                    'data', ""data""
                );
            END;
            $BODY$;
            ";
        public static async Task SeedAsync(AppDbContext dbContext, 
            IConfiguration configuration,
            ILogger logger,
            RoleManager<IdentityRole<int>> roleMgr,
            UserManager<AppUser> userMgr)
        {
            try
            {
                if (dbContext.Database.IsNpgsql())
                {
                    // Check for pending migrations
                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        // Apply migrations
                        await dbContext.Database.MigrateAsync();

                        //Load functions, sprocs
                        await dbContext.Database.ExecuteSqlRawAsync(loadSalesSprocSql);
                        await dbContext.Database.ExecuteSqlRawAsync(getSearch);
                        await dbContext.Database.ExecuteSqlRawAsync(searchSql);

                        //Add roles
                        var rolesSection = configuration.GetSection("Roles").GetChildren();
                        var roles = rolesSection.Select(role => role.Value).ToArray();
                        foreach (var rol in roles)
                        {
                            var roleExists = await roleMgr.RoleExistsAsync(rol);
                            if (!roleExists)
                            {
                                var newRole = new IdentityRole<int>(rol);

                                await roleMgr.CreateAsync(newRole);
                            }
                        }

                        //Add admin(s)
                        var adminsSection = configuration.GetSection("Admins");
                        var admins = adminsSection.Get<List<AppUser>>();
                        foreach (var entry in admins)
                        {
                            var userExists = await userMgr.FindByEmailAsync(entry.Email) != null ||
                                await userMgr.FindByNameAsync(entry.UserName) != null;

                            if (!userExists)
                            {
                                var password = entry.PasswordHash;
                                entry.PasswordHash = null;
                                await userMgr.CreateAsync(entry, password);
                            }

                            var user = await userMgr.FindByEmailAsync(entry.Email);
                            var isAdmin = await userMgr.IsInRoleAsync(user, "Admin");
                            if (!isAdmin)
                            {
                                await userMgr.AddToRoleAsync(user, "Admin");
                            }
                        }
                    }
                    //Load counties
                    if (!dbContext.Counties.AsNoTracking().Any())
                    {
                        await LoadCounties(dbContext, logger);
                    }
                }  

            }
            catch (Exception ex)
            {
                logger.LogError("Error in the SeedAsync method: {@Exception}", ex);
            }
        }

        private static async Task LoadCounties(AppDbContext dbContext, ILogger logger) 
        {
            var path = Path.Join(AppContext.BaseDirectory, "Infrastructure/Data/Resources/county_state.xlsx");
            try
            {
                // Register the code page provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int rowCount = 0;
                        int batchSize = 100;
                        // Skip the header row
                        if (reader.Read())
                        {
                            var batch = new List<County>();

                            while (reader.Read())
                            {

                                rowCount++;
                                // Read the current row into an array
                                var rowValues = new object[reader.FieldCount];
                                reader.GetValues(rowValues);
                                if (rowValues == null)
                                    continue;

                                var county = new County(
                                    Convert.ToInt32(rowValues[0]),
                                    Convert.ToString(rowValues[1]),
                                    Convert.ToString(rowValues[2])
                                );

                                batch.Add(county);


                                // Process batch
                                if (rowCount % batchSize == 0)
                                {
                                    //Process batch logic
                                    if (batch.Count > 0)
                                    {
                                        dbContext.Counties.AddRange(batch);
                                        await dbContext.SaveChangesAsync(); 
                                        batch.Clear();
                                    }
                                }
                            }

                            // Process any remaining rows not forming a full batch
                            if (batch.Count > 0)
                            {
                                dbContext.Counties.AddRange(batch);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error loading counties: @{Exception} ", ex);
            }
        }
    }
}



