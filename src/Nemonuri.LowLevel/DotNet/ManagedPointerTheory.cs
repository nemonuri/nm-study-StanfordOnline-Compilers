
using System.Diagnostics;
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.DotNet;

// .NET Runtime specific theory

public static class ManagedPointerTheory
{
    public static unsafe ObjectOrPointerReference ToObjectOrPointerReference<T>(ref DuckTypedProperty<ObjectOrPointer, T> locationProvider)
    {
        static ObjectOrPointer SelectorImpl(ObjectOrPointer boxedLocationProvider)
        {
            ref var locProvider = ref boxedLocationProvider.DangerousUnbox<DuckTypedProperty<ObjectOrPointer, T>>();
            ref var loc = ref locProvider.InvokeProperty();
            return locProvider.Receiver.IsFixed ? new(Unsafe.AsPointer(ref Unsafe.As<T?, byte>(ref loc))) : new(loc!);
        }

        return new(locationProvider.Receiver, new(&SelectorImpl));
    }

    public static ObjectOrPointerReference ToObjectOrPointerReference<T>
    (
        ref T location,
        FunctionHandle<T, DuckTypedProperty<ObjectOrPointer, T>> inverseLocationProvider
    )
    {
        ref var locationProvider = ref inverseLocationProvider.InvokeFunction(ref location!);
        return ToObjectOrPointerReference(ref locationProvider);
    }

    public static ObjectOrPointerReference ToObjectOrPointerReference<T>
    (
        ref T location,
        DuckTypedMethod<ObjectOrPointer, T, DuckTypedProperty<ObjectOrPointer, T>> inverseLocationProvider
    )
    {
        ref var locationProvider = ref inverseLocationProvider.InvokeMethod(ref location!);
        return ToObjectOrPointerReference(ref locationProvider);
    }
}
