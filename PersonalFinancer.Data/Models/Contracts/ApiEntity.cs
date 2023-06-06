namespace PersonalFinancer.Data.Models.Contracts
{
    public abstract class ApiEntity
    {
        public abstract string Id { get; set; }

        public abstract string Name { get; set; }

        public abstract string OwnerId { get; set; }

        public abstract bool IsDeleted { get; set; }
    }
}
