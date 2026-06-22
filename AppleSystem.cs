
using Cecs;
using Cecs.Systems;
using Raylib_cs;

namespace CatchApple;

public static class AppleSystem
{
    public static int RevictApples (
        Store<Position> positionStore, 
        Store<Obtainable> obtainableStore,
        List<World.Entity> buffer,
        int worldWSize
    )
    {
        int lostApples = 0;
        buffer.GetEntitiesWith(obtainableStore).And(positionStore);
        for (int i = 0; i < buffer.Count; i++)
        {
            ref var item = ref positionStore.Components[buffer[i].Id];
            if (item.Point.Y > worldWSize)
            {
                lostApples++;
                positionStore.RemoveEntity(buffer[i]);
            }
        }

        return lostApples;
    }

    public static void RecicleApples(
        List<World.Entity> buffer, 
        Store<Position> positionStore,
        Store<Velocity> velocityStore,
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
            GenerateApple(geometryStore, obtainableStore, positionStore, velocityStore, buffer, (int)world.defaultSize.X, ref speed);
        }
    }

    static void GenerateApple (Store<Geometry> geometryStore, Store<Obtainable> obtainableStore, Store<Position> positionStore, Store<Velocity> velocityStore, List<World.Entity> workBuffer, int worldWSize, ref float speed)
    {
        workBuffer.GetEntitiesWith(velocityStore).And(obtainableStore).AndNo(positionStore);
        if (workBuffer.Count <= 0) return;

        positionStore.AddEntity(workBuffer[^1], new ()
        {
            Point = GetRandomApplePosition(geometryStore.GetComponent(workBuffer[^1]), worldWSize)
        });

        velocityStore.Components[workBuffer[^1].Id].Point = new(0, speed);
    }

    static Vec2 GetRandomApplePosition (Geometry geometry, int screenWidth)
    {
        float scaledWidth = geometry.Point.X;
        var X = Random.Shared.Next(0, screenWidth - (int)scaledWidth);
        var Y = -geometry.Point.Y;

        return new(X, Y);
    }
}