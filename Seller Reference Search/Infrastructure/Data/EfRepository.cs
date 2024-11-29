using Ardalis.Specification.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Infrastructure.Interfaces;


public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
