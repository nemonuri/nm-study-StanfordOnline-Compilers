namespace DscTool.Scoped.Hashtables;

public static class ValueEntryDictionaryTheory
{
    extension<T, TCondition, TCategory, TDictionary>
    (scoped ref readonly ReadOnlyTypeBox<(T, TCondition, TCategory), TDictionary> theory)
        where TCategory : IScopedCategory<T, TCondition>
        where TDictionary : IValueEntryDictionary<T, TCondition, TCategory>
    {
        public static
        ReadOnlyTypeBox<(T, TCondition, TCategory), TDictionary>
        Theories(scoped ref readonly TDictionary source) => 
        TypeBox.ReadOnlyBox<(T, TCondition, TCategory), TDictionary>(in source);
    }
}