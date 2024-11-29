using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seller_Reference_Search.Models.DataTable;
using Seller_Reference_Search.Models;
using MediatR;
using Seller_Reference_Search.Models.Commands;
using Newtonsoft.Json;

namespace Seller_Reference_Search.Controllers
{
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IMediator _mediator;

        public UploadController(ILogger<UploadController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public IActionResult Index()
        {
            var isAdmin = User.IsInRole("Admin");
            ViewData["isAdmin"] = isAdmin;

            var model = (string)TempData["FileUploadDto"];
            return View(model != null ? JsonConvert.DeserializeObject<FileUploadDto>(model): new FileUploadDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FileUploadDto model)
        {
            var isAdmin = User.IsInRole("Admin");

            if (ModelState.IsValid)
            {
                var cmd = new UploadSalesCommand(model);
                var result = await _mediator.Send(cmd);
                TempData["FileUploadDto"] = JsonConvert.SerializeObject(result);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetUploads(DtParameters<FileUploadResultDto> dtParams)
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

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUploads([FromBody] int[] ids)
        {
            try
            {
                var deleteCmd = new DeleteUploadsCommand(ids);
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
