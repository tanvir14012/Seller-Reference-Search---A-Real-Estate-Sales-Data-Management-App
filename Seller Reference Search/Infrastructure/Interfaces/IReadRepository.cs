using Ardalis.Specification;

namespace Seller_Reference_Search.Infrastructure.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
