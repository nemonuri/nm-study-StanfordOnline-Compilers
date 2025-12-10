namespace DscTool;

public interface ITreePremise<TLeveledSemiGroup>
{
    ReadOnlySpan<TLeveledSemiGroup> Decompose(scoped ref readonly TLeveledSemiGroup source);
}
