using Cecs.Systems;

namespace Cecs.Components;
public record struct Rendereable<T>(T Texture2D, Vec2 Size) : IComponent;
