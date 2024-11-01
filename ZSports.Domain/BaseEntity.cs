namespace ZSports.Domain
{
    public abstract class BaseEntity<TKey>
    {
        public virtual TKey? Id { get; set; } = default;
    }
}
