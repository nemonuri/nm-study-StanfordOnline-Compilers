
namespace Nemonuri.LowLevel;

public interface IConfig<TShared, TIndividual, TConfig> 
    where TShared : class
    where TConfig : IConfig<TShared, TIndividual, TConfig>
{
    TShared SharedConfig {get;}

    [UnscopedRef] ref readonly TIndividual IndividualConfig {get;}
    
    TConfig WithNewIndividualConfig(in TIndividual individual);
}
