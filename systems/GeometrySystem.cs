using Cecs;

namespace CatchApple;
public readonly record struct Vec2(float X, float Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator <(Vec2 one, Vec2 other) => one.X < other.X && one.Y < other.Y;
    public static bool operator >(Vec2 one, Vec2 other) => one.X > other.X && one.Y > other.Y;
}

public interface IHasPosition2D
{
    Vec2 Point { get; set; }
}

public interface IHasSize2D
{
    Vec2 Point { get; set; }
}

public interface IHasVelocity2D
{
    Vec2 Point { get; set; }
}

public record struct Position(Vec2 Point) : IComponent, IHasPosition2D;
public record struct Velocity(Vec2 Point) : IComponent, IHasVelocity2D;
public record struct Geometry(Vec2 Point) : IComponent, IHasSize2D;
public static class GeometrySystem
{
    public static void Move <P, V>
    (Store<P> positionStore, Store<V> velocityStore, List<Entity> workBuffer)
    where P : struct, IComponent, IHasPosition2D
    where V : struct, IComponent, IHasVelocity2D
    {
        workBuffer.GetEntitiesWith(velocityStore).And(positionStore);
        for (int i = 0; i < workBuffer.Count; i++)
        {
            var pos = positionStore.Components[workBuffer[i].Id].Point;
            var acc = velocityStore.Components[workBuffer[i].Id].Point;
            var newPos = pos + acc;
            positionStore.Components[workBuffer[i].Id] = positionStore.Components[workBuffer[i].Id] with
            {
                Point = newPos
            };
        }
    }
}