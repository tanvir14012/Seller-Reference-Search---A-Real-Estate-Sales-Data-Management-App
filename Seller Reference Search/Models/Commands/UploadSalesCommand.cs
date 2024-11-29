using MediatR;

namespace Seller_Reference_Search.Models.Commands
{
    public class UploadSalesCommand: IRequest<FileUploadDto>
    {
        public FileUploadDto FileUploadDto { get; set; }
        public UploadSalesCommand(FileUploadDto model)
        {
            FileUploadDto = model;
        }
    }
}
