namespace eVybir.Repos
{
    public record DbWrapped<TKey, TEntity>(TKey Key, TEntity Entity);
}
