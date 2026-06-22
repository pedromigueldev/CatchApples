namespace Cecs.Systems;
public record struct Rendereable<T>(T Texture2D, Vec2 Size, float Scale) : IComponent;

public delegate void RendereableCallBack<T, P> (Rendereable<T> rendereable, P position)
where P : struct, IComponent, IHasPosition2D;
public static class RenderSystem
{
    public static void RenderEntities<T, P>(
        this Store<Rendereable<T>> textureStore,
        Store<P> positionStore,
        RendereableCallBack<T, P> rendereableCallBack,
        List<World.Entity> workBuffer
    )
    where P : struct, IComponent, IHasPosition2D
    {
        workBuffer.GetEntitiesWith(positionStore).And(textureStore);
        for (int i = 0; i < workBuffer.Count; i++)
        {
            var entity = workBuffer[i].Id;
            var pos = positionStore.Components[entity];
            rendereableCallBack(textureStore.Components[entity], pos);
        }
    }
}