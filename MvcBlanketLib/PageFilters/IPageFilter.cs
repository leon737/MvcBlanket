namespace MvcBlanketLib.PageFilters
{
    public interface IPageFilter<out T>
    {
        T Value { get; }
    }
}
