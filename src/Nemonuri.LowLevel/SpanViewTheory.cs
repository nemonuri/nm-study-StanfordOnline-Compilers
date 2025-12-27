namespace Nemonuri.LowLevel;

public static class SpanViewTheory
{
    public static bool TryGetSpanView<T, TView>(object? owner, scoped ref SpanView<T, TView> spanView)
    {
        if (owner is ISpanViewOwner<T, TView> ensuredOwner)
        {
            ensuredOwner.GetSpanView(ref spanView);
            return true;
        }
        return false;
    }

    public static bool TryGetSpanView<T, TView>(this ISpanViewProvider<TView> owner, scoped ref SpanView<T, TView> spanView) =>
        TryGetSpanView((object)owner, ref spanView);
}
