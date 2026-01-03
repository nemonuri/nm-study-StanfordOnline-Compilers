
using Nemonuri.LowLevel.DuckTyping;

namespace Nemonuri.LowLevel;

public partial class DangerousMemoryViewProviderBuilder<TReceiverComponent, TArgumentComponent> : 
    IDangerousMemoryViewProviderBuilder<int, DangerousMemoryViewProviderBuilder<TReceiverComponent, TArgumentComponent>.Provider, TReceiverComponent, TArgumentComponent>
    where TReceiverComponent : IEquatable<TReceiverComponent>
    where TArgumentComponent : IEquatable<TArgumentComponent>
{
    // Note: 자동 구현 속성은, 참조로 반환 불가...아쉽네.
    private readonly Shared _sharedState;
    public ref readonly Shared SharedState => ref _sharedState;

    public DangerousMemoryViewProviderBuilder()
    {
        _sharedState = new Shared(new(4),new(4),new(4),new(4));
    }

    public int Length => _sharedState.ProviderProviders.Length;

    public ref LowLevelKeyValuePair<int, Provider> this[int index] => ref _sharedState.ProviderProviders[index];


    public int AddReceiverComponent(TReceiverComponent receiverComponent)
    {
        TypeHint<(TReceiverComponent, ArrayViewBuilder<TReceiverComponent>)> th = default;
        _sharedState.Providers.TryAddAndGetIndex(in receiverComponent, out var key ,th);
        return key;
    }

    public int AddArgumentComponent(TArgumentComponent argumentComponent)
    {
        TypeHint<(TArgumentComponent, ArrayViewBuilder<TArgumentComponent>)> th = default;
        _sharedState.Arguments.TryAddAndGetIndex(in argumentComponent, out var key ,th);
        return key;
    }

    public unsafe int AddHandleComponent<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent) where TMemoryView : IMemoryView<T>
    {
        TypedUnmanagedBox<nint> addingHandle = TypedUnmanagedBox<nint>.Box(in handleComponent);

        static bool EqImpl(in TypedUnmanagedBox<nint> l, in TypedUnmanagedBox<nint> r) => l.BoxedValue == r.BoxedValue;

        TypeHint<(TypedUnmanagedBox<nint>, ArrayViewBuilder<TypedUnmanagedBox<nint>>)> th = default;

        _sharedState.GetMemoryViewHandles.TryAddAndGetIndex(in addingHandle, new(&EqImpl), out var key, th);
        return key;
    }

    public unsafe LowLevelKeyValuePair<int, Provider> BuildProvider(int receiver, int argument, int handle)
    {
        TypeHint<(LowLevelKeyValuePair<int, Provider>,PackedTable<int, Provider>.Builder)> th = default;

        static bool EqImpl(in LowLevelKeyValuePair<int, Provider> l, in LowLevelKeyValuePair<int, Provider> r) =>
               l.Value.ArgumentIndex == r.Value.ArgumentIndex
            && l.Value.MethodHandleIndex == r.Value.MethodHandleIndex
            && l.Value.ProviderIndex == r.Value.ProviderIndex
            ;

        bool found = _sharedState.ProviderProviders.TryAddAndGetIndex(new(0,new(default!,receiver,argument,handle)),new(&EqImpl),out var key,th);
        if (found)
        {
            return _sharedState.ProviderProviders[key];
        }

        int nextKey = _sharedState.ProviderProviders.Length;
        LowLevelKeyValuePair<int, Provider> newEntry = new(nextKey, new(this, receiver, argument, handle));
        return newEntry;
    }
}
