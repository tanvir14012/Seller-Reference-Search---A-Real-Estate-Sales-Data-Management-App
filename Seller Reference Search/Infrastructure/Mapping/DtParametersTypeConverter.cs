using AutoMapper;
using Seller_Reference_Search.Models.DataTable;

namespace Seller_Reference_Search.Infrastructure.Mapping
{
    public class DtParametersTypeConverter<TSource, TDestination> : ITypeConverter<DtParameters<TSource>, DtParameters<TDestination>>
    {
        public DtParameters<TDestination> Convert(DtParameters<TSource> source, DtParameters<TDestination> destination, ResolutionContext context)
        {
            return new DtParameters<TDestination>
            {
                Draw = source.Draw,
                Start = source.Start,
                Length = source.Length,
                Columns = source.Columns,
                Order = source.Order,
                Search = source.Search,
                AdditionalValues = source.AdditionalValues
            };
        }
    }

}
