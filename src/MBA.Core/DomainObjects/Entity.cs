namespace MBA.Core.DomainObjects;

public abstract class Entity
{
	public Guid Id { get; set; }

	public Entity()
	{
		Id = Guid.NewGuid();
	}

	public override string ToString()
	{
		return $"{GetType().Name} [Id={Id}]";
	}
}