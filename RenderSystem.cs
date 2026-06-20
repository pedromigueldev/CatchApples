
using CatchApple.Geometry;
using Cecs;

namespace CatchApple.Render;
public record struct Rendereable<T>(T Texture2D, Vec2 Size) : IComponent;
