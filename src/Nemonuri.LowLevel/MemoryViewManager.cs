
namespace Nemonuri.LowLevel;

public partial class MemoryViewManager<TBuilderReceiver, TBuilderArgument> : 
    IAddableDangerousMemoryViewProviderTable<int, MemoryViewManager<TBuilderReceiver, TBuilderArgument>.Provider, TBuilderReceiver, TBuilderArgument>
    where TBuilderReceiver : IEquatable<TBuilderReceiver>
    where TBuilderArgument : IEquatable<TBuilderArgument>
{
    // Note: 사실상 유일한 Field 가 Box<Shared> 뿐이니, 그냥 MemoryViewManager 를 Class 로 만들어도 되는 것 아닌가?
    // public Box<Shared> SharedConfig {get;}

    // Note: 자동 구현 속성은, 참조로 반환 불가...아쉽네.
    private readonly Shared _sharedState;
    public ref readonly Shared SharedState => ref _sharedState;

    public MemoryViewManager()
    {
        _sharedState = new Shared(new(4),new(4),new(4),new(4));
    }

    public int Length => _sharedState.ProviderProviders.Length;

    public ref LowLevelKeyValuePair<int, Provider> this[int index] => ref _sharedState.ProviderProviders[index];


    public unsafe void Add<T, TMemoryView>
    (
        TBuilderReceiver memoryViewProvider, 
        TBuilderArgument memoryViewArgument, 
        MethodHandle<TBuilderReceiver, TBuilderArgument, TMemoryView> getMemoryViewHandle
    )
        where TMemoryView : IMemoryView<T>
    {
        ref readonly var shared = ref _sharedState;

        // Note
        // - MemoryViewConfig.Provider 는, memoryViewProviderProvider 가 되는 것인가...;;
        // - 뭔가 구현하려다 보면, ReferenceReference 나, ProviderProvider 같은 게 꼭 생긴 단 말이지;;
        // - ...그래. 그냥 MemoryViewConfig.ProviderProvider 라고, 대놓고 지르자!

        AddableMemoryViewTheory.Theorize<TBuilderReceiver, ArrayViewBuilder<TBuilderReceiver>>(in shared.Providers).TryAddAndGetIndex(in memoryViewProvider, out var providerIndex);
        AddableMemoryViewTheory.Theorize<TBuilderArgument, ArrayViewBuilder<TBuilderArgument>>(in shared.Arguments).TryAddAndGetIndex(in memoryViewArgument, out var argumentIndex);

        TypedUnmanagedBox<nint> addingHandle = TypedUnmanagedBox<nint>.Box(in getMemoryViewHandle);
        static bool EqImpl(in TypedUnmanagedBox<nint> l, in TypedUnmanagedBox<nint> r) => l.BoxedValue == r.BoxedValue;
        AddableMemoryViewTheory.Theorize<TypedUnmanagedBox<nint>, ArrayViewBuilder<TypedUnmanagedBox<nint>>>(in shared.GetMemoryViewHandles).TryAddAndGetIndex(in addingHandle, new(&EqImpl), out var methodHandleIndex);

        int nextKey = shared.ProviderProviders.Length;
        shared.ProviderProviders.Add(new(nextKey, new(this, providerIndex, argumentIndex, methodHandleIndex)));
    }
}
