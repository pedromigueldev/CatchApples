using cs_ecs;

namespace CatchApple;
public delegate void RendereableCallBack<T, P> (T rendereable, P position, float scale)
    where P : struct, IComponent, IHasPosition2D;

public static class RenderSystem
{
    public static void RenderEntities<T, P>(
        Archetype<(T Texture2D, P Size, float Scale)> renderable,
        RendereableCallBack<T, P> rendereableCallBack
    )
    where P : struct, IComponent, IHasPosition2D
    {
        for (int i = 0; i < renderable.Entities.Count; i++)
        {
            var tex = renderable.ComponentsTuple[i].Texture2D;
            var pos = renderable.ComponentsTuple[i].Size;
            var scl = renderable.ComponentsTuple[i].Scale;
            rendereableCallBack(tex, pos, scl);
        }
    }
}