using Ardalis.Specification;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models.DataTable;
using System.Linq.Expressions;

namespace Seller_Reference_Search.Models.Infrastructure.Specs
{
    public class FileUploadFilterSpec: Specification<FileUpload>
    {
        public FileUploadFilterSpec(DtParameters<FileUpload> dtParameters)
        {

            if (!string.IsNullOrEmpty(dtParameters.Search?.Value))
            {
                ApplySearch(dtParameters.Search.Value, dtParameters.Columns);
            }

            if (dtParameters.Order != null && dtParameters.Order.Length > 0)
            {
                ApplySorting(dtParameters.Order, dtParameters.Columns);
            }

            ApplyPaging(dtParameters.Start, dtParameters.Length);
        }

        private void ApplyPaging(int start, int length)
        {
            Query.Skip(start).Take(length);
        }

        private void ApplySearch(string searchValue, DtColumn[] columns)
        {
            var parameter = Expression.Parameter(typeof(FileUpload), "x");
            Expression orExpression = null;

            foreach (var column in columns)
            {
                if (column.Searchable)
                {
                    var property = Expression.Property(parameter, column.Data);

                    // Convert property to lowercase
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(property, toLowerMethod);

                    // Convert searchValue to lowercase
                    var searchValueLower = searchValue.ToLower();

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var searchExpression = Expression.Call(propertyToLower, containsMethod, Expression.Constant(searchValueLower));

                    if (orExpression == null)
                    {
                        orExpression = searchExpression;
                    }
                    else
                    {
                        orExpression = Expression.OrElse(orExpression, searchExpression);
                    }
                }
            }

            if (orExpression != null)
            {
                var lambda = Expression.Lambda<Func<FileUpload, bool>>(orExpression, parameter);
                Query.Where(lambda);
            }


        }

        private void ApplySorting(DtOrder[] order, DtColumn[] columns)
        {
            var parameter = Expression.Parameter(typeof(FileUpload), "x");
            var column = columns[order[0].Column];
            var property = Expression.Property(parameter, column.Data);

            var lambda = Expression.Lambda<Func<FileUpload, object>>(Expression.Convert(property, typeof(object)), parameter);

            if (order[0].Dir == DtOrderDir.Desc)
            {
                Query.OrderByDescending(lambda);
            }
            else
            {
                Query.OrderBy(lambda);
            }
        }
    }
}
