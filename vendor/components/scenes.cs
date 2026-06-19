namespace Cecs.Components;

public record struct Space(Vec2 Point) : Cecs.IComponent
{
    public static Space New(int X, int Y)
    {
        return new Space (new (X, Y));
    }
};
