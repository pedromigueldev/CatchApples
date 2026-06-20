namespace Cecs.Systems;
public record struct Player() : IComponent;
public static class PlayerSystem
{
    public enum PlayerMove
    {
        Up,
        Down,
        Left,
        Right,
        Still
    }

    public static World.Entity New(World world, Vec2 Size)
    {
        var pos = new Geometry();
        var center = pos.Size.X / 4;
        var posX = (world.defaultSize.X / 2) - center;
        pos.Size = Size;
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
        var point = new Vec2(0,0);

        if (playerMove == PlayerMove.Up)    point = new Vec2(0, -1);
        if (playerMove == PlayerMove.Left)  point = new Vec2(-1, 0);
        if (playerMove == PlayerMove.Down)  point = new Vec2(0, 1);
        if (playerMove == PlayerMove.Right) point = new Vec2(1, 0);
        pos.Velocity = new (point);
    }
}