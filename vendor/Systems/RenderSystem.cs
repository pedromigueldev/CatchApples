namespace Cecs.Systems;
public record struct Rendereable<T>(T Texture2D, Vec2 Size) : IComponent;
