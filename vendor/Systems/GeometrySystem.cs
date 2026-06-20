namespace Cecs.Systems;

public readonly record struct Vec2(float X, float Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static bool operator <(Vec2 one, Vec2 other) => one.X < other.X && one.Y < other.Y;
    public static bool operator >(Vec2 one, Vec2 other) => one.X > other.X && one.Y > other.Y;
}

public readonly record struct Position(Vec2 Point)
{
    public static Vec2 operator +(Position a, Vec2 b) => new(a.Point.X + b.X, a.Point.Y + b.Y);
};
public readonly record struct Velocity(Vec2 Point);
public record struct Geometry(Position Position, Velocity Velocity, Vec2 Size) : IComponent;
public static class GeometrySystem
{
    public static void Move(Store<Geometry> store)
    {
        for (int i = 0; i < store.Entities.Count; i++)
        {;
            var pos = store.Components[store.Entities[i].Id].Position;
            var acc = store.Components[store.Entities[i].Id].Velocity;
            var newPos = new Position(pos.Point + acc.Point);

            store.Components[store.Entities[i].Id] = store.Components[store.Entities[i].Id] with
            {
                Position = newPos
            };
        }
    }
}