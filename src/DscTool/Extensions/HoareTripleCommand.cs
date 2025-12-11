namespace DscTool.Extensions;

public static class HoareTripleCommand
{
    extension<T, TPredicate>(IHoareTripleCommand<T, TPredicate> command)
    {
        public bool TryInvoke<TChecker>
        (
            TChecker checker,
            T source,
            [NotNullWhen(true)] out T? result,
            out TPredicate? postCondition
        )
            where TChecker : IPredicatePremise<T, TPredicate>
        {
            ref readonly var commandTheory = 
                ref HoareTripleTheory.TheorizeCommand<T, TPredicate, IHoareTripleCommand<T, TPredicate>>(in command);
            
            var briefed = commandTheory.InvokeAndBrief(in checker, in source, out postCondition);
            if (briefed.IsOk)
            {
                result = briefed.GetOk();
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
