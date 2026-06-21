
using Cecs;
using Cecs.Systems;
using Raylib_cs;

namespace CatchApple;

public static class AppleSystem
{
    public static void RevictApples (
        List<World.Entity> buffer,
        Store<Geometry> geometryStore, 
        Store<Obtainable> obtainableStore,
        World world,
        ref int lostApples
    )
    {
        buffer.GetEntitiesWith(obtainableStore).And(geometryStore);
        for (int i = 0; i < buffer.Count; i++)
        {
            ref var item = ref geometryStore.GetComponent(buffer[i]);
            if (item.Position.Point.Y > world.defaultSize.Y)
            {
                lostApples++;
                geometryStore.RemoveEntity(buffer[i]);
            }
        }
    }

    public static void RecicleApples(
        List<World.Entity> buffer, 
        Texture2D appleTexture,
        Store<Geometry> geometryStore, 
        Store<Obtainable> obtainableStore, 
        World world,
        float tick,
        ref float timer,
        ref float speed
    )
    {
        timer += Raylib.GetFrameTime();
        if (timer >= tick)
        {
            timer -= tick;
            GenerateApple(appleTexture,buffer, geometryStore, obtainableStore, world, ref speed);
        }
    }

    static void GenerateApple (Texture2D appleTexture, List<World.Entity> buffer, Store<Geometry> geometryStore, Store<Obtainable> obtainableStore, World world, ref float speed)
    {
        buffer.GetEntitiesWith(obtainableStore).AndNo(geometryStore);
        if (buffer.Count <= 0) return;
        geometryStore.AddEntity(buffer[^1], new Geometry()
        {
            Position = new (GetRandomApplePosition(appleTexture, (int)world.defaultSize.X)),
            Velocity = new (new(0, speed))
        });
    }

    static Vec2 GetRandomApplePosition (Texture2D appleTexture, int screenWidth)
    {
        const float appleScale = 0.2f;
        float scaledWidth = appleTexture.Width;

        var X = Random.Shared.Next(0, screenWidth - (int)scaledWidth);
        var Y = -appleTexture.Height * appleScale;

        return new(X, Y);
    }
}