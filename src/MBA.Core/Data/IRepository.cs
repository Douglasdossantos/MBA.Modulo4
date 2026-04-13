using MBA.Core.DomainObjects;

namespace MBA.Core.Data;

public interface IRepository<T> : IDisposable where T : IAggregateRoot
{
	IUnitOfWork UnitOfWork { get; }
}