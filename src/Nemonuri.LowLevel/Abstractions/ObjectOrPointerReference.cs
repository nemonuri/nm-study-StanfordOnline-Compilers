using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.Abstractions;

public readonly struct ObjectOrPointerReference
{
    public unsafe readonly static FunctionHandle DefaultFunctionHandle = new(&Primitives.PureFunctionTheory.Identity);

    public readonly ObjectOrPointer Base;
    public readonly FunctionHandle SelectorHandle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointerReference(ObjectOrPointer @base, FunctionHandle selectorHandle)
    {
        Base = @base;
        SelectorHandle = selectorHandle.IsNull ? DefaultFunctionHandle : selectorHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer Dereference() => SelectorHandle.InvokeFunction(Base);

    public ObjectOrPointerReference ComposeAnotherSelectorHandle(FunctionHandle anotherSelectorHandle) => ComposeAnotherSelectorHandleCore(this, anotherSelectorHandle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static ObjectOrPointerReference ComposeAnotherSelectorHandleCore(ObjectOrPointerReference baseReference, FunctionHandle anotherSelectorHandle)
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
        public readonly ObjectOrPointerReference BaseReference;
        public readonly FunctionHandle AnotherSelectorHandle;

        public SelectorImplSource(ObjectOrPointerReference baseReference, FunctionHandle anotherSelectorHandle)
        {
            BaseReference = baseReference;
            AnotherSelectorHandle = anotherSelectorHandle.IsNull ? DefaultFunctionHandle : anotherSelectorHandle;
        }
    }

    // Note : 혹시, 'Fixed box' 를 할 수 있을까?
    public ObjectOrPointer ToObjectOrPointer() => new(this);
}
