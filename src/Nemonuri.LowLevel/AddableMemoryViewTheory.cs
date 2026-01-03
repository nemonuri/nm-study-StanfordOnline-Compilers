
using System.Diagnostics;

namespace Nemonuri.LowLevel;

public static class AddableMemoryViewTheory
{
    extension<T, TMemoryView>
    (scoped in TheoryBox<T, TMemoryView> theory)
        where TMemoryView : IMemoryView<T>, IAddable<T>
    {
        public static
        TheoryBox<T, TMemoryView>
        Theorize(in TMemoryView source)
        =>
        TheoryBox.Box
        <T, TMemoryView>
        (in source);

        public bool TryAddAndGetIndex
        (
            in T adding,
            RelationHandle<T, T> equivalenceRelation,
            out int resultIndex
        )
        {
            ref readonly var view = ref theory.Self;
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
    (scoped in TheoryBox<T, TMemoryView> theory)
        where T : IEquatable<T>
        where TMemoryView : IMemoryView<T>, IAddable<T>
    {
        public unsafe bool TryAddAndGetIndex
        (
            in T adding, 
            out int resultIndex
        )
        {
            return theory.TryAddAndGetIndex(in adding, new(&DotNet.ManagedPointerTheory.AreEquivalent), out resultIndex);
        }
    }
}

