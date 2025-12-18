namespace DscTool.Scoped.Hashtables;

public static class ScopedCategoryProvidableDictionaryTheory
{
    extension<T, TCondition, TCategory, TDictionary>
    (scoped ref readonly ReadOnlyTypeBox<(T, TCondition, TCategory), TDictionary> theory)
        where TCategory : IScopedCategory<T, TCondition>
        where TDictionary : IScopedCategoryProvidableDictionary<T, TCondition, TCategory>
    {
        public static
        ReadOnlyTypeBox<(T, TCondition, TCategory), TDictionary>
        Theorize(scoped ref readonly TDictionary source) => 
        TypeBox.ReadOnlyBox<(T, TCondition, TCategory), TDictionary>(in source);

        public bool TryGetCategoryFromKey(scoped ref readonly T key, [NotNullWhen(true)] scoped ref TCategory? category)
        {
            scoped ref readonly var self = ref theory.Self;

            if (!self.TryGetValue(key, out var ve)) {return false;}
            if (!self.TryGetCategoryFromCondition(in ve.Condition, ref category)) {return false;}

            return true;
        }

    }
}