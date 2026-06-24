namespace Cecs.Systems;
public record struct Rendereable<T>(T Texture2D, Vec2 Size, float Scale) : IComponent;

public delegate void RendereableCallBack<T, P> (T rendereable, P position, float scale)
where P : struct, IComponent, IHasPosition2D;
public static class RenderSystem
{
    public static void RenderEntities<T, P>(
        Archetype<(P, float, T)> renderable,
        RendereableCallBack<T, P> rendereableCallBack
    )
    where P : struct, IComponent, IHasPosition2D
    {
        for (int i = 0; i < renderable.Entities.Count; i++)
        {
            var pos = renderable.ComponentsTuple[i].Item1;
            var scl = renderable.ComponentsTuple[i].Item2;
            var tex = renderable.ComponentsTuple[i].Item3;
            rendereableCallBack(tex, pos, scl);
        }
    }
}