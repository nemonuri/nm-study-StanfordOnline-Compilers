
namespace Nemonuri.LowLevel;

public static class AddableMemoryViewTheory
{
    extension<T, TMemoryView>
    (TMemoryView view)
        where TMemoryView : IMemoryView<T>, IAddable<T>
    {
        public bool TryAddAndGetIndex
        (
            in T adding,
            RelationHandle<T, T> equivalenceRelation,
            out int resultIndex,
            TypeHint<(T, TMemoryView)> th = default
        )
        {
            int length = view.Length;
            int i;
            for (i = 0; i < length; i++)
            {
                if (equivalenceRelation.InvokeRelation(in view[i], in adding))
                {
                    resultIndex = i;
                    return false;
                }
            }

            view.Add(in adding);

            resultIndex = i;
            return true;
        }
    }

    extension<T, TMemoryView>
    (TMemoryView view)
        where T : IEquatable<T>
        where TMemoryView : IMemoryView<T>, IAddable<T>
    {
        public unsafe bool TryAddAndGetIndex
        (
            in T adding, 
            out int resultIndex,
            TypeHint<(T, TMemoryView)> th = default
        )
        {
            return view.TryAddAndGetIndex(in adding, new(&DotNet.ManagedPointerTheory.AreEquivalent), out resultIndex);
        }
    }
}

