
namespace DscTool;

public interface ISemiGroup<T>
{
    void Compose(scoped ref readonly T left, scoped ref readonly T right, scoped ref T dest);
}
