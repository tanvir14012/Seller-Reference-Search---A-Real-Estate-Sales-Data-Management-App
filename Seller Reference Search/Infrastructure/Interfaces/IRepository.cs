using Ardalis.Specification;

namespace Seller_Reference_Search.Infrastructure.Interfaces;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
