using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Reference
{
    public unsafe readonly static FunctionHandle DefaultFunctionHandle = new(&Primitives.PureFunctionTheory.Identity);

    public readonly ObjectOrPointer Base;
    public readonly FunctionHandle SelectorHandle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Reference(ObjectOrPointer @base, FunctionHandle selectorHandle)
    {
        Base = @base;
        SelectorHandle = selectorHandle.IsNull ? DefaultFunctionHandle : selectorHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer Dereference() => SelectorHandle.InvokeFunction(Base);

    public T DangerousCast<T>() => Dereference().DangerousCast<T>();

    public Reference ComposeAnotherSelectorHandle(FunctionHandle anotherSelectorHandle) => ComposeAnotherSelectorHandleCore(this, anotherSelectorHandle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static Reference ComposeAnotherSelectorHandleCore(Reference baseReference, FunctionHandle anotherSelectorHandle)
    {
        // 다음 식이 참이 되도록, result 를 만들어야 한다.
        // anotherSelectorHandle.InvokeFunction(baseReference.Dereference()) == result.Dereference()

        static ObjectOrPointer SelectorImpl(ObjectOrPointer boxedSource)
        {
            SelectorImplSource source = boxedSource.DangerousUnbox<SelectorImplSource>();
            return source.AnotherSelectorHandle.InvokeFunction(source.BaseReference.Dereference());
        }

        SelectorImplSource source = new(baseReference, anotherSelectorHandle);
        ObjectOrPointer o = new(source);
        return new(o,new(&SelectorImpl));
    }

    private readonly struct SelectorImplSource
    {
        public readonly Reference BaseReference;
        public readonly FunctionHandle AnotherSelectorHandle;

        public SelectorImplSource(Reference baseReference, FunctionHandle anotherSelectorHandle)
        {
            BaseReference = baseReference;
            AnotherSelectorHandle = anotherSelectorHandle.IsNull ? DefaultFunctionHandle : anotherSelectorHandle;
        }
    }

    // Note
    // - 혹시, 'Fixed Pointer box' 를 할 수 있을까?
    // - 그렇다면, IsObjectOrPointerReference 를 어떻게 고쳐야 하지?
    // - ...아니, 아무리 생각해봐도 답 없어. 결국 '동적으로 증가하는 메모리 공간'이 필요해.
    public ObjectOrPointer ToObjectOrPointer() => new(this);

}
