using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using System.Data;

namespace Seller_Reference_Search.Features
{
    public class PrepareSearchHandler : IRequestHandler<PrepareSearchCommand, SearchOptionsDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public PrepareSearchHandler(AppDbContext dbContext,
            ILogger<PrepareSearchHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<SearchOptionsDto> Handle(PrepareSearchCommand request, CancellationToken cancellationToken)
        {
            var searchOps = new SearchOptionsDto();
            try
            {
                using (var connection = _dbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT public.prepare_search()";
                        command.CommandType = CommandType.Text;
                        var result = await command.ExecuteScalarAsync();
                        searchOps.json = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error preparing search model", ex);
                searchOps.Errored = true;
                searchOps.Message = "Something went wrong! Please try again later.";
            }

            return searchOps;
        }
    }
}
