namespace Cecs.Systems;
public record struct Rendereable<T>(T Texture2D, Vec2 Size, float Scale) : IComponent;

public delegate void RendereableCallBack<T> (Rendereable<T> rendereable, Position position);
public static class RenderSystem
{
    public static void RenderEntities<T>(
        this Store<Rendereable<T>> textureStore,
        List<World.Entity> buffer, 
        Store<Geometry> geometryStore,
        RendereableCallBack<T> rendereableCallBack
    )
    {
        var items = WorldImpl.GetEntitiesWith(buffer, geometryStore).And(textureStore);
        for (int i = 0; i < items.Count; i++)
        {
            var entity = items[i];

            ref var pos = ref geometryStore.GetComponent(entity);
            {
                rendereableCallBack(textureStore.GetComponent(entity), pos.Position);
            }
        }
    }
}