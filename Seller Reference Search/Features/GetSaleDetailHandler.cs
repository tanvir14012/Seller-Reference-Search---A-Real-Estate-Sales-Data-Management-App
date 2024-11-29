using MediatR;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using System.Globalization;

namespace Seller_Reference_Search.Features
{
    public class GetSaleDetailHandler : IRequestHandler<GetSaleDetailCommand, SaleDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetSaleDetailHandler> _logger;

        public GetSaleDetailHandler(AppDbContext dbContext,
            ILogger<GetSaleDetailHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<SaleDto> Handle(GetSaleDetailCommand request, CancellationToken cancellationToken)
        {
            try
            {

                return await (from sale in _dbContext.Sales
                              where sale.Id == request.Id
                              select new SaleDto
                              {
                                  Id = sale.Id,
                                  OwnerName = sale.OwnerName,
                                  Reference = sale.Reference,
                                  ParcelNumber = sale.ParcelNumber,
                                  LotAcreage = Math.Round(sale.LotAcreage, 2),
                                  OfferPrice = Math.Round(sale.OfferPrice, 2).ToString("C", new CultureInfo("en-US", false)).Replace(".00", ""),
                                  OfferPPA = Math.Round(sale.OfferPPA ?? decimal.Zero, 2).ToString("C", new CultureInfo("en-US", false)),
                                  RealPPA = Math.Round(sale.RealPPA ?? decimal.Zero, 2).ToString("C", new CultureInfo("en-US", false)),
                                  PPACalc = Math.Round(sale.PPACalc ?? decimal.Zero, 2).ToString("C", new CultureInfo("en-US", false)),
                                  Profit = Math.Round(sale.Profit ?? decimal.Zero, 2).ToString("C", new CultureInfo("en-US", false)).Replace(".00", ""),
                                  RetailValue = Math.Round(sale.RetailValue ?? decimal.Zero, 2).ToString("C", new CultureInfo("en-US", false)).Replace(".00", ""),
                                  ClosingDate = sale.ClosingDate,
                                  County = sale.County,
                                  State = sale.State,
                                  ZipCode = sale.ZipCode,
                                  LastModifiedAt = sale.LastModifiedAt
                              }).FirstOrDefaultAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading a sale detail", ex);
            }

            return new SaleDto();
        }
    }
}
