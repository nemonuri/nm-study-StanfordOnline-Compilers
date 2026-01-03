namespace Nemonuri.Graph.LowLevel;

public interface ILowLevelGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> :
    IMemoryView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface ILowLevelGraph<TNodeKey, TEdgeLabel, TNodeValue, TMemoryView, TReceiver> :
    ILowLevelTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>, TMemoryView>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
    where TMemoryView : IMemoryView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
}
