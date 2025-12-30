namespace Nemonuri.LowLevel;

public static class BoxThoery
{
    extension<T, TBox>
    (scoped in TheoryBox<T, TBox> theory)
        where TBox : IBox<T>
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        public static
        TheoryBox<T, TBox>
        Theorize(in TBox source)
        =>
        TheoryBox.Box
        <T, TBox>
        (in source);

        public ReadOnlyBoxReceiver<TBox, T> ToReadOnlyBox()
        {
            static T? ValueGetter(ref TBox receiver) => receiver.Value;

            unsafe { return new(theory.Self, new(&ValueGetter)); }
        }
    }
}

public static class RefBoxThoery
{
    extension<T, TRefBox>
    (scoped in TheoryBox<T, TRefBox> theory)
        where TRefBox : IRefBox<T>
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        public static
        TheoryBox<T, TRefBox>
        Theorize(in TRefBox source)
        =>
        TheoryBox.Box
        <T, TRefBox>
        (in source);

        public BoxReceiver<TRefBox, T> ToBox()
        {
            static T? ValueGetter(ref TRefBox receiver) => receiver.Value;
            static void ValueSetter(ref TRefBox receiver, in T? value) => receiver.Value = value;

            unsafe { return new(theory.Self, new(&ValueGetter, &ValueSetter)); }
        }
    }
}