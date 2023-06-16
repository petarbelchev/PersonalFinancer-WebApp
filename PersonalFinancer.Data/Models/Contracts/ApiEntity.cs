namespace PersonalFinancer.Data.Models.Contracts
{
    public abstract class ApiEntity : CacheableEntity
    {
        public abstract Guid Id { get; set; }
    }
}
