using AutoMapper;
using MediatR;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Infrastructure.Interfaces;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.DataTable;
using Seller_Reference_Search.Models.Infrastructure.Specs;

namespace Seller_Reference_Search.Features
{
    public class UserGetRequestHandler : IRequestHandler<DtParameters<AppUserDto>, DtResult<AppUserDto>>
    {
        private readonly IReadRepository<AppUser> _appUserRepository;
        private readonly IMapper _mapper;

        public UserGetRequestHandler(IReadRepository<AppUser> appUserRepository,
            IMapper mapper)
        {
            _appUserRepository = appUserRepository;
            _mapper = mapper;
        }
        public async Task<DtResult<AppUserDto>> Handle(DtParameters<AppUserDto> requestParams, CancellationToken cancellationToken)
        {
            var mappedDtParams = _mapper.Map<DtParameters<AppUser>>(requestParams);
            var filterSpec = new AppUserFilterSpec(mappedDtParams);
            var totalRecords = await _appUserRepository.CountAsync(cancellationToken);
            var filteredRecords = await _appUserRepository.CountAsync(filterSpec, cancellationToken);
            var users = await _appUserRepository.ListAsync(filterSpec, cancellationToken);
            var data = users.Select(x => _mapper.Map<AppUserDto>(x)).ToArray();

            return new DtResult<AppUserDto>
            {
                Draw = requestParams.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}
