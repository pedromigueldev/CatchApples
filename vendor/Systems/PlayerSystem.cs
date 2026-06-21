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

    public static World.Entity New(World world, Vec2 Size)
    {
        var pos = new Geometry();
        var center = Size.X / 4;
        var posX = (world.defaultSize.X / 2) - center;
        pos.Position = pos.Position with
        {
            Point = new(posX, 0)
        };

        World.Entity entity = world.CreateEntity()
            .AddComponent<Player>(world)
            .AddComponent<Geometry>(world);

        return entity;
    }

    public static void Move(World.Entity entity, Store<Geometry> store, PlayerMove playerMove)
    {
        ref var pos = ref store.GetComponent(entity);
        int x = 0;
        int y = 0;
        if ((playerMove & PlayerMove.Up) != 0)      y -= 1;
        if ((playerMove & PlayerMove.Down) != 0)    y += 1;
        if ((playerMove & PlayerMove.Left) != 0)    x -= 1;
        if ((playerMove & PlayerMove.Right) != 0)   x += 1;
        pos.Velocity = new (new (x, y));
    }
}