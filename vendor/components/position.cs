namespace Cecs.Components;

public readonly record struct Vec2(float X, float Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
}
public readonly record struct Position(Vec2 Point)
{
    public static Vec2 operator +(Position a, Vec2 b) => new(a.Point.X + b.X, a.Point.Y + b.Y);
};
public readonly record struct Velocity(Vec2 Point);
public record struct Phase(Position Position, Velocity Velocity, Vec2 Size) : IComponent;