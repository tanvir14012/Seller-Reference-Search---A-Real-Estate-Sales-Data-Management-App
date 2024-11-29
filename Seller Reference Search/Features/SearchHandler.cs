using MediatR;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Models.Commands;
using Seller_Reference_Search.Models.DataTable;
using System.Data;
using System.Text;

namespace Seller_Reference_Search.Features
{
    public class SearchHandler : IRequestHandler<SearchSaleCommand, string>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;
        private static string sql = @"
			SELECT 
			""Id"" AS ""id"",
			""Reference"" AS ""reference"",
			""OwnerName"" AS ""ownerName"",
			""ParcelNumber"" AS ""parcelNumber"",
			ROUND(CAST(""LotAcreage"" AS NUMERIC), 2) AS ""lotAcreage"",
			TO_CHAR(""OfferPrice"", 'FM$999,999,999.00') AS ""offerPrice"",
            TO_CHAR(""OfferPPA"", 'FM$999,999,999.00') AS ""offerPPA"",
            TO_CHAR(""RealPPA"", 'FM$999,999,999.00') AS ""realPPA"",
            ROUND(CAST(""PPACalc"" AS NUMERIC), 2) AS ""ppaCalc"",
            TO_CHAR(""Profit"", 'FM$999,999,999.00') AS ""profit"",
            TO_CHAR(""RetailValue"", 'FM$999,999,999.00') AS ""retailValue"",
			""County"" AS ""county"",
			""State"" AS ""state"",
			""ZipCode"" AS ""zipCode"",
			to_char(""ClosingDate"",'MM/DD/YYYY') AS ""closingDate"",
			to_char(""CreatedAt"", 'MM/DD/YYYY') AS ""createdAt"",
			to_char(""LastModifiedAt"", 'MM/DD/YYYY') AS ""lastModifiedAt""
		FROM public.""Sales""
		{0} 
        {1} 
        {2}
		";
        private static string resultCountSql = @"SELECT COUNT(*) FROM public.""Sales"" {0}";

        public SearchHandler(AppDbContext dbContext,
            ILogger<PrepareSearchHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<string> Handle(SearchSaleCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var orderColumn = "LastModifiedAt";

                switch (request.Model.Order[0]?.Column ?? -1)
                {
                    case 0:
                        orderColumn = "OwnerName";
                        break;
                    case 1:
                        orderColumn = "Reference";
                        break;
                    case 2:
                        orderColumn = "ParcelNumber";
                        break;
                    case 3:
                        orderColumn = "LotAcreage";
                        break;
                    case 4:
                        orderColumn = "OfferPrice";
                        break;
                    case 5:
                        orderColumn = "OfferPPA";
                        break;
                    case 6:
                        orderColumn = "RealPPA";
                        break;
                    case 7:
                        orderColumn = "PPACalc";
                        break;
                    case 8:
                        orderColumn = "Profit";
                        break;
                    case 9:
                        orderColumn = "RetailValue";
                        break;
                    case 10:
                        orderColumn = "ClosingDate";
                        break;
                    case 11:
                        orderColumn = "County";
                        break;
                    case 12:
                        orderColumn = "State";
                        break;
                    case 13:
                        orderColumn = "ZipCode";
                        break;
                }

                var orderByClause = string.Format("ORDER BY {0} {1}", $"\"{orderColumn}\"", request.Model.Order[0].Dir == DtOrderDir.Asc ? "ASC" : "DESC");
                var paginationClause = string.Format("LIMIT {0} OFFSET {1}", request.Model.Length, request.Model.Start);

                double.TryParse(request.Form["acreage_min"], out double acreageMin);
                double.TryParse(request.Form["acreage_max"], out double acreageMax);

                decimal.TryParse(request.Form["offer_min"], out decimal offerMin);
                decimal.TryParse(request.Form["offer_max"], out decimal offerMax);

                var filterClause = new StringBuilder();
                var oneFilterIsApplied = false;

                if (!string.IsNullOrWhiteSpace(request.Model.Search.Value))
                {
                    filterClause.Append(string.Format("{0} (\"Reference\" ILIKE '%{1}%' OR \"OwnerName\" ILIKE '%{2}%' OR \"ParcelNumber\" ILIKE '%{3}%') ",
                        oneFilterIsApplied ? "AND " : "WHERE ", request.Model.Search.Value.Trim(), request.Model.Search.Value.Trim(), request.Model.Search.Value.Trim()));
                    oneFilterIsApplied = true;
                }

                if (acreageMin != 0 || request.Form["acreage_ref_max"] != request.Form["acreage_max"])
                {
                    filterClause.Append(string.Format("{0} (\"LotAcreage\" BETWEEN {1} AND {2}) ", oneFilterIsApplied ? "AND " : "WHERE ", acreageMin, acreageMax));
                    oneFilterIsApplied = true;
                }

                if (offerMin != 0 || request.Form["offer_ref_max"] != request.Form["offer_max"])
                {
                    filterClause.Append(string.Format("{0} (\"OfferPrice\" BETWEEN {1} AND {2}) ", oneFilterIsApplied ? "AND " : "WHERE ", offerMin, offerMax));
                    oneFilterIsApplied = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Form["county"]) && request.Form["county"].ToString() != "Any")
                {
                    filterClause.Append(string.Format("{0} (\"County\" = '{1}')  ", oneFilterIsApplied ? "AND " : "WHERE ", request.Form["county"].ToString().Trim()));
                    oneFilterIsApplied = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Form["state"]) && request.Form["state"].ToString() != "Any")
                {
                    filterClause.Append(string.Format("{0} (\"State\" = '{1}')  ", oneFilterIsApplied ? "AND " : "WHERE ", request.Form["state"].ToString().Trim()));
                    oneFilterIsApplied = true;
                }

                var searchQuery = new StringBuilder()
                    .AppendFormat(sql,filterClause.ToString(), orderByClause, paginationClause)
                    .ToString();

                var filteredResultCountQuery = string.Format(resultCountSql, filterClause.ToString());

                //Connect to db and execute
                using (var connection = _dbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT public.search_sales(@sql, @cnt, @draw)";
                        command.CommandType = CommandType.Text;

                        // Add parameters
                        var textParam = command.CreateParameter();
                        textParam.ParameterName = "@sql";
                        textParam.Value = searchQuery;
                        command.Parameters.Add(textParam);

                        var textParam2 = command.CreateParameter();
                        textParam2.ParameterName = "@cnt";
                        textParam2.Value = filteredResultCountQuery;
                        command.Parameters.Add(textParam2);

                        var intParam = command.CreateParameter();
                        intParam.ParameterName = "@draw";
                        intParam.Value = request.Model.Draw;
                        command.Parameters.Add(intParam);


                        var data = await command.ExecuteScalarAsync();
                        return data.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the search execution: ", ex);
            }

            return null;
        }
    }
}
