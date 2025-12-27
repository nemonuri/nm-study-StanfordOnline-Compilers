
namespace Nemonuri.LowLevel;

public readonly ref partial struct SpanView<T, TView>
{
    public ref struct Enumerator
    {
        private readonly SpanView<T, TView> _parent;
        private int _index;

        internal Enumerator(SpanView<T, TView> parent)
        {
            _parent = parent;
            _index = -1;
        }

        public readonly ref TView Current => ref _parent[_index];

        public bool MoveNext()
        {
            _index += 1;
            return _index < _parent.Length;
        }
    }
}
