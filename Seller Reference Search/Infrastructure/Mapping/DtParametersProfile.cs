using AutoMapper;
using Seller_Reference_Search.Models.DataTable;

namespace Seller_Reference_Search.Infrastructure.Mapping
{
    public class DtParametersProfile: Profile
    {
        public DtParametersProfile()
        {
            CreateMap(typeof(DtParameters<>), typeof(DtParameters<>)).ConvertUsing(typeof(DtParametersTypeConverter<,>));
        }
    }
}
