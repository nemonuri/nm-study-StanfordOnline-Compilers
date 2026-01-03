
namespace Nemonuri.LowLevel.DuckTyping;

public interface IMemoryViewProviderFactory<T, TMemoryView, TReceiver, TArgument>
    where TMemoryView : IMemoryView<T>
{
    MemoryViewDuckTypedProvider<T, TMemoryView, TReceiver, TArgument> ToProvider();
}
