
using Cecs;
using Cecs.Systems;
using Raylib_cs;

namespace CatchApple;

public static class AppleSystem
{
    public static int RevictApples (
        Store<Obtainable> obtainableStore, Store<Position> positionStore, Store<Geometry> geometryStore,
        Store<Out> outStore,
        List<Entity> buffer,
        int worldWSize
    )
    {
        buffer.GetEntitiesWith(obtainableStore).AndNo(outStore);
        int lostApples = 0;
        for (int i = 0; i < buffer.Count; i++)
        {
            ref var item = ref positionStore.Components[buffer[i].Id];
            if (item.Point.Y > worldWSize)
            {
                lostApples++;
                positionStore.Components[buffer[i].Id].Point = 
                    GetRandomApplePosition(geometryStore.GetComponent(buffer[i]), worldWSize);
                outStore.AddEntity(buffer[i], new ());
            }
        }

        return lostApples;
    }

    public static void GenerateApple (Store<Obtainable> obtainableStore,
                                      Store<Position> positionStore,
                                      Store<Geometry> geometryStore,
                                      Store<Velocity> velocityStore,
                                      Store<Out> outStore,
                                      List<Entity> workBuffer,
                                      int worldWSize,
                                      ref float speed)
    {
        workBuffer.GetEntitiesWith(obtainableStore).And(outStore);
        if (workBuffer.Count <= 0) return;
        positionStore.Components[workBuffer[^1].Id].Point = GetRandomApplePosition(geometryStore.GetComponent(workBuffer[^1]), worldWSize);
        velocityStore.Components[workBuffer[^1].Id].Point = new(0, speed);
        outStore.RemoveEntity(workBuffer[^1]);
    }

    static Vec2 GetRandomApplePosition (Geometry geometry, int screenWidth)
    {
        float scaledWidth = geometry.Point.X;
        var X = Random.Shared.Next(0, screenWidth - (int)scaledWidth);
        var Y = -geometry.Point.Y;

        return new(X, Y);
    }
}