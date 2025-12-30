
namespace Nemonuri.LowLevel;

public static class SingleOrSequenceProviderTheory
{
    extension<T, TSequence, TProvider>
    (scoped in TheoryBox<(T, TSequence), TProvider> theory)
        where TSequence : IMemoryView<T>
        where TProvider : ISingleOrMemoryViewProvider<T, TSequence>
    {
        public static
        TheoryBox<(T, TSequence), TProvider>
        Theorize
        (in TProvider source)
        =>
        TheoryBox.Box
        <(T, TSequence), TProvider>
        (in source);

        public int GetSingleOrSequenceWithLength
        (
            out T? single, 
            out TSequence? sequence
        )
        {
            single = default;
            sequence = default;
            if (theory.Self.GetSingleOrMemory(ref single, ref sequence))
            {
                return 1;
            }
            else
            {
                return sequence.Length;
            }
        }
    }

    const int Eq = 0;
    const int Lt = -1;
    const int Gt = 1;

    extension<T, TSequence, TProvider>
    (scoped in TheoryBox<(T, TSequence), TProvider> theory)
        where TSequence : IMemoryView<T>
        where TProvider : ISingleOrMemoryViewProvider<T, TSequence>
        where T : ISingleOrMemoryViewProvider<T, TSequence>
    {

        private int CompareSequence<TComparer>
        (
            scoped in T? leftSingle, scoped in TSequence? leftSequence, int leftLength,
            scoped in T? rightSingle, scoped in TSequence? rightSequence, int rightLength,
            scoped in TComparer leafComparer
        )
            where TComparer : ILowLevelComparer<T>
        {
            if (!(leftLength > 1 && rightLength > 1))
            {
                return (leftLength >= 1, rightLength >= 1) switch
                {
                    (false, false) => Eq,
                    (true, true) => 
                        (leftLength == 1, rightLength == 1) switch
                        {
                            (true, true) => theory.CompareTree(in leftSingle!, in rightSingle!, in leafComparer),
                            (true, false) => theory.CompareTree(in leftSingle!, in rightSequence![0], in leafComparer),
                            (false, true) => theory.CompareTree(in leftSequence![0], in rightSingle!, in leafComparer),
#pragma warning disable CS0436
                            (false, false) => throw new SwitchExpressionException((false, false))
#pragma warning restore CS0436
                        },
                    (false, true) => Lt,
                    (true, false) => Gt
                };
            }

            int maxLength = Math.Max(leftLength, rightLength);
            for (int i = 0; i < maxLength; i++)
            {
                if (!(i < leftLength && i < rightLength))
                {
                    return leftLength.CompareTo(rightLength);
                }

                int compareResult = theory.CompareTree(in leftSequence![i], in rightSequence![i], in leafComparer);
                if (compareResult is not Eq) { return compareResult; }
            }

            return Eq;
        }

        public int CompareTree<TComparer>(scoped in T left, scoped in T right, scoped in TComparer leafComparer)
            where TComparer : ILowLevelComparer<T>
        {
            int leftChildrenLength = Theorize<T, TSequence, T>(in left).GetSingleOrSequenceWithLength(out var leftChild, out var leftChildren);
            int rightChildrenLength = Theorize<T, TSequence, T>(in right).GetSingleOrSequenceWithLength(out var rightChild, out var rightChildren);

            if (leftChildrenLength is 0 && rightChildrenLength is 0) { return leafComparer.Compare(in left, in right); }

            return theory.CompareSequence
            (
                in leftChild, in leftChildren, leftChildrenLength,
                in rightChild, in rightChildren, rightChildrenLength,
                in leafComparer
            );
        }
    }
}

public delegate void SingleOrSequenceAction<T, TSequence>(scoped ref T? single, scoped ref TSequence? sequence, int length)
    where TSequence : IMemoryView<T>;