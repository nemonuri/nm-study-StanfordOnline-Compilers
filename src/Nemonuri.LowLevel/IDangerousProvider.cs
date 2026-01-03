
namespace Nemonuri.LowLevel;

public interface IDangerousProviderArity1
{
    ref T1 DangerousGet<T1>();
}

public interface IDangerousProviderArity2
{
// Note: '이름'으로 구분하기 위해서는, '항 수'를 쓰는 수 밖에 없겠지...?

    ref T2 DangerousGet<T1, T2>();
}