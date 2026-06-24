using Cecs;

namespace CatchApple;
public record struct Player() : IComponent;
public static class PlayerSystem
{
    [Flags]
    public enum PlayerMove
    {
        None  = 0,
        Up    = 1 << 0, // 1
        Down  = 1 << 1, // 2
        Left  = 1 << 2, // 4
        Right = 1 << 3, // 8
    }

    public static void Move<V> (Entity entity, Store<V> velocityStore, PlayerMove playerMove, float playerSpeed)
    where V : struct, IComponent, IHasVelocity2D
    {
        float x = 0;
        float y = 0;
        if ((playerMove & PlayerMove.Up) != 0)      y -= playerSpeed;
        if ((playerMove & PlayerMove.Down) != 0)    y += playerSpeed;
        if ((playerMove & PlayerMove.Left) != 0)    x -= playerSpeed;
        if ((playerMove & PlayerMove.Right) != 0)   x += playerSpeed;
        velocityStore.Components[entity.Id].Point = new (x, y);
    }
}