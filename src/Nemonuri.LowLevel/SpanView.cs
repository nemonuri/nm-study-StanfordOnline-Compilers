
using static Nemonuri.LowLevel.Extensions.SpanExtensions;

namespace Nemonuri.LowLevel;

public readonly ref partial struct SpanView<T, TView> : ISpanView<TView>
{
    private readonly Span<T> _span;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public SpanView(Span<T> span, RefSelectorHandle<T, TView> selectorHandle)
    {
        _span = span;
        _selectorHandle = selectorHandle;
    }

    public int Length => _span.Length;

    public ref TView this[int index]
    {
        get
        {
            Guard.IsInRange(index, 0, Length);

            return ref _selectorHandle.Invoke(ref _span[index]);
        }
    }

    public Enumerator GetEnumerator() => new(this);

    private bool TryCopyToCore<T2>(in SpanView<T2, TView> dest, bool throwIfError)
    {
        if (!(Length <= dest.Length))
        {
            if (throwIfError)
            {
                ThrowHelper.ThrowArgumentException("destination length should be greater than or equal to source length.");
            }

            return false;
        }

        for (int i = 0; i < Length; i++)
        {
            dest[i] = this[i];
        }
        return true;
    }
    public bool TryCopyTo<T2>(in SpanView<T2, TView> dest) => TryCopyToCore(in dest, throwIfError: false);
    public void CopyTo<T2>(in SpanView<T2, TView> dest) => TryCopyToCore(in dest, throwIfError: true);


    private bool TryCopyToCore(Span<TView> dest, bool throwIfError) => TryCopyToCore(dest.ToIdentityView(), throwIfError);
    public bool TryCopyTo(Span<TView> dest) => TryCopyToCore(dest, throwIfError: false);
    public void CopyTo(Span<TView> dest) => TryCopyToCore(dest, throwIfError: true);


    public TView[] ToArray()
    {
        var destArray = new TView[Length];
        CopyTo(destArray.AsSpan());
        return destArray;
    }

}
