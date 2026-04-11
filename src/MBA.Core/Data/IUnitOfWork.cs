namespace MBA.Core.Data;

public interface IUnitOfWork
{
	Task<bool> Commit();
}