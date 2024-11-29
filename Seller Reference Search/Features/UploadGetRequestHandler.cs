using AutoMapper;
using MediatR;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Infrastructure.Interfaces;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.DataTable;
using Seller_Reference_Search.Models.Infrastructure.Specs;

namespace Seller_Reference_Search.Features
{
    public class UploadGetRequestHandler : IRequestHandler<DtParameters<FileUploadResultDto>, DtResult<FileUploadResultDto>>
    {
        private readonly IReadRepository<FileUpload> _fileUploadRepository;
        private readonly IMapper _mapper;

        public UploadGetRequestHandler(IReadRepository<FileUpload> FileUploadRepository,
            IMapper mapper)
        {
            _fileUploadRepository = FileUploadRepository;
            _mapper = mapper;
        }
        public async Task<DtResult<FileUploadResultDto>> Handle(DtParameters<FileUploadResultDto> requestParams, CancellationToken cancellationToken)
        {
            var mappedDtParams = _mapper.Map<DtParameters<FileUpload>>(requestParams);
            var filterSpec = new FileUploadFilterSpec(mappedDtParams);
            var totalRecords = await _fileUploadRepository.CountAsync(cancellationToken);
            var filteredRecords = await _fileUploadRepository.CountAsync(filterSpec, cancellationToken);
            var users = await _fileUploadRepository.ListAsync(filterSpec, cancellationToken);
            var data = users.Select(x => _mapper.Map<FileUploadResultDto>(x)).ToArray();

            return new DtResult<FileUploadResultDto>
            {
                Draw = requestParams.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}
