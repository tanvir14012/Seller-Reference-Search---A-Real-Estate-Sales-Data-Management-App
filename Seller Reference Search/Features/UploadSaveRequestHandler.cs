using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Infrastructure.Interfaces;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using Seller_Reference_Search.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace PrivilegePro.Features
{
    public class UploadSaveRequestHandler : IRequestHandler<UploadSalesCommand, FileUploadDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHost;
        private readonly ILogger<UploadSaveRequestHandler> _logger;
        private readonly IRepository<FileUpload> _fileUploadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExcelService _excelService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public UploadSaveRequestHandler(AppDbContext dbContext,
            UserManager<AppUser> userManager,
            IMapper mapper,
            IWebHostEnvironment webHost,
            ILogger<UploadSaveRequestHandler> logger,
            IRepository<FileUpload> fileUploadRepository,
            IHttpContextAccessor httpContextAccessor,
            ExcelService excelService,
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
            _webHost = webHost;
            _logger = logger;
            _fileUploadRepository = fileUploadRepository;
            _httpContextAccessor = httpContextAccessor;
            _excelService = excelService;
            _backgroundTaskQueue = backgroundTaskQueue;
        }
        public async Task<FileUploadDto> Handle(UploadSalesCommand request, CancellationToken cancellationToken)
        {
            var file = request.FileUploadDto.File;
            var timeStart = DateTime.Now;

            if (file == null || file.Length == 0)
            {
                return new FileUploadDto
                {
                    Errored = true,
                    Message = "The file is received either empty or malformed."
                };
            }

            try
            {
                // Create a folder path if it doesn't exist
                var uploadsFolderPath = Path.Combine(_webHost.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                // Append timestamp to the file name
                var timestamp = timeStart.ToString("yyyyMMddHHmmssfff");
                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                // Save the file to the target path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                }

                var fileUpload = new FileUpload
                {
                    UploadedByUserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    FileSize = (int)Math.Ceiling((double)file.Length/(1024*1024)),
                    UploadPath = filePath,
                    FileName = fileName,
                    Status = "In-progress",
                    UploadDuration = DateTime.Now - timeStart
                };

                await _fileUploadRepository.AddAsync(fileUpload);
                await _fileUploadRepository.SaveChangesAsync();

                //Queue the background task
                try
                {
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (serviceProvider, token) =>
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var excelService = scope.ServiceProvider.GetRequiredService<ExcelService>();
                            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ExcelService>>();

                            await excelService.ProcessExcelFileAsync(fileUpload, dbContext, logger, token);
                        }
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error enqueuing background processing task", ex);
                }

                return new FileUploadDto
                {
                    Message = "The file has been uploaded and processing has started"
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Upload error: {@Exception}", ex);

                return new FileUploadDto
                {
                    Errored = true,
                    Message = "Unable to save the file to the server/database. Please check the file access permissions and ensure that the server is configured properly."
                };
            }

            
        }

    }
}
