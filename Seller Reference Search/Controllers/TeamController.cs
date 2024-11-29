using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using Seller_Reference_Search.Models.DataTable;

namespace Seller_Reference_Search.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamController: Controller
    {
        private readonly ILogger<TeamController> _logger;
        private readonly IMediator _mediator;

        public TeamController(ILogger<TeamController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers(DtParameters<AppUserDto> dtParams)
        {
            try
            {
                var result = await _mediator.Send(dtParams);

                // Return the data as JSON
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("@{Exception}", ex);
                return StatusCode(500, $"Error! Please try again later.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUser([FromForm] AppUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var saveCmd = new SaveUserCommand(model);
                    var result = await _mediator.Send(saveCmd);

                    // Return the data as JSON
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError("@{Exception}", ex);
                    return StatusCode(500, $"Error! Please try again later.");
                }

            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                                   .Select(x => x.ErrorMessage)
                                   .ToArray();
            return StatusCode(500, new
            {
                success = false,
                errors
            });

        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers([FromBody] int[] ids)
        {
            try
            {
                var deleteCmd = new DeleteUsersCommand(ids);
                var result = await _mediator.Send(deleteCmd);

                // Return the data as JSON
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("@{Exception}", ex);
                return StatusCode(500, $"Error! Please try again later.");
            }
        }
    }
}
