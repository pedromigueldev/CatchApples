namespace Cecs.Systems;
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

    public static void Move(World.Entity entity, Store<Geometry> store, PlayerMove playerMove)
    {
        ref var pos = ref store.GetComponent(entity);
        int x = 0;
        int y = 0;
        if ((playerMove & PlayerMove.Up) != 0)      y -= 10;
        if ((playerMove & PlayerMove.Down) != 0)    y += 10;
        if ((playerMove & PlayerMove.Left) != 0)    x -= 10;
        if ((playerMove & PlayerMove.Right) != 0)   x += 10;
        pos.Velocity = new (new (x, y));
    }
}