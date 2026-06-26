using cs_ecs;
using Raylib_cs;

namespace CatchApple;
public record struct Render(Texture2D Texture2D) : IComponent;
public delegate void RendereableCallBack<T, P> (T rendereable, P position, float scale)
    where P : struct, IComponent, IHasPosition2D;

public static class RenderSystem
{
    public static void RenderEntities(With<Render, Position> renderable, RendereableCallBack<Render, Position> rendereableCallBack)
    {
        foreach (var (render, position, index) in renderable)
        {
            rendereableCallBack(render, position, 1f);   
        }
    }
}