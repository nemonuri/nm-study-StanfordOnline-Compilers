
namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TProvider, TBuilderReceiver, TBuilderArgument> :
    IMemoryView<LowLevelKeyValuePair<TKey, TProvider>>,
    IDangerousMemoryViewProviderComponentAddable<TBuilderReceiver, TBuilderArgument>
    where TProvider : IDangerousMemoryViewProvider
{
}

// Note: DangerousProvider 와, FreeTypeAddable ...이 둘을 만들어 주어야 하는군.

public interface IDangerousMemoryViewProvider
{
    [UnscopedRef] ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> ;
}

public interface IDangerousMemoryViewProviderComponentAddable<TBuilderReceiver, TBuilderArgument>
{
    // ...문제는, TEntry 타입이 T, TMemoryView 타입에 의존적이라는 것이다.
    // 'Or' 타입 연산은 못 하는 C# 컴파일러의 한계...하긴, 할 수 있었으면 컴파일 속도가 엄청 느렸겠지;;
    public void Add<T, TMemoryView>
    (
        TBuilderReceiver memoryViewProviderBuilderReceiver, 
        TBuilderArgument memoryViewProviderBuilderArgument, 
        MethodHandle<TBuilderReceiver, TBuilderArgument, TMemoryView> memoryViewProviderBuilderHandle
    ) where TMemoryView : IMemoryView<T> ;

    // 이 arguments 들을 '조립' 하면, MemoryViewProvider 가 나온다...!
    // 즉, provider 의 '빌더' 를 Add 하는 셈이네....
    // - 음, '빌더'를 add 한다기 보다는, 'Component' 를 add 한다고 봐야 하지 않을까.
}
