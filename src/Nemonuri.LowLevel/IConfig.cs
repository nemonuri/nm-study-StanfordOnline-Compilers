
namespace Nemonuri.LowLevel;

public interface IConfig<TShared, TIndividual> 
    where TShared : class
{
    TShared SharedConfig {get;}

    [UnscopedRef] ref readonly TIndividual IndividualConfig {get;}
}
