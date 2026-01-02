
using Nemonuri.LowLevel.Abstractions;

namespace Nemonuri.LowLevel;

public struct UnmanagedValueProvider<T> where T : unmanaged
{
    private readonly RuntimeTypeHandle _typeHandle;
    private ObjectOrPointer _provider;
    private readonly RefSelectorHandle<ObjectOrPointer, T> _refSelectorHandle;

    public UnmanagedValueProvider(RuntimeTypeHandle typeHandle, ObjectOrPointer provider, RefSelectorHandle<ObjectOrPointer, T> refSelectorHandle)
    {
        _typeHandle = typeHandle;
        _provider = provider;
        _refSelectorHandle = refSelectorHandle;
    }

/*
    public ref T2 DangerousGetValue<T2>() where T2 : unmanaged
    {
        
    }
*/
}
