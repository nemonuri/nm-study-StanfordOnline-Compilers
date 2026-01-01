namespace Nemonuri.Graph.LowLevel;

public interface ILowLevelGraph<TNodeKey, TEdgeLabel, TNodeValue, TMemoryView, TConfig> :
    ILowLevelTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>, TMemoryView>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
    where TMemoryView : IMemoryView<RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>>>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
}
