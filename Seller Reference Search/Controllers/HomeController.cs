using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using Seller_Reference_Search.Models.DataTable;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace Seller_Reference_Search.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,
            IMediator mediator,
            AppDbContext dbContext)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var cmd = new PrepareSearchCommand(new SearchOptionsDto());
            var model = await _mediator.Send(cmd);    
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DtParameters<SaleDto> dtParameters)
        {
           
            try
            {
                var cmd = new SearchSaleCommand(dtParameters, Request.Form);
                var data = await _mediator.Send(cmd);
                if(data != null) 
                    return Ok(data);
                return StatusCode(500, $"Something is not right! Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError("@{Exception}", ex);
                return StatusCode(500, $"Error! Please try again later.");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cmd = new GetSaleDetailCommand(id);
            var model = await _mediator.Send(cmd);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetCounties([FromBody] string stateCode)
        {

            try
            {
                if(string.IsNullOrWhiteSpace(stateCode)
                    || stateCode.ToLower().Trim() == "any")
                {
                    return Ok(new string[] {});
                }

                var counties = await (from c in _dbContext.Counties.AsNoTracking()
                                    join s in _dbContext.Sales.AsNoTracking()
                                        on c.CountyName equals s.County
                                    where (c.StateCode == stateCode.ToUpper())
                                        && c.StateCode == s.State
                                    select c.CountyName).Distinct().OrderBy(c => c).ToListAsync();

                return Ok(counties);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in getcountries: @{Exception}", ex);
                return StatusCode(500, $"Error! Please try again later.");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
