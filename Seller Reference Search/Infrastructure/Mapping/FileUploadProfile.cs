using AutoMapper;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models;

namespace Seller_Reference_Search.Infrastructure.Mapping
{
    public class FileUploadProfile: Profile
    {
        public FileUploadProfile()
        {
            CreateMap<FileUpload, FileUploadResultDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                //.ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
                //.ForMember(dest => dest.UploadPath, opt => opt.MapFrom(src => src.UploadPath))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UploadDuration, opt => opt.MapFrom(src => src.UploadDuration))
                //.ForMember(dest => dest.RowsFound, opt => opt.MapFrom(src => src.RowsFound))
                //.ForMember(dest => dest.RowsInserted, opt => opt.MapFrom(src => src.RowsInserted))
                //.ForMember(dest => dest.RowsUpdated, opt => opt.MapFrom(src => src.RowsUpdated))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy hh:mm:ss tt")));
        }
    }
}
