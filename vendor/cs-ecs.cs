namespace Cecs;
internal interface IStore
{
    int MaxValue { get; }
    World.Entity[] Entities { get; init; }
    int HighestEntityId { get; }
    Type ComponentType { get; }
}

internal class Store<T> : IStore where T : struct, IComponent {
    public required int MaxValue { get; set; }   
    public required World.Entity[] Entities { get; init; }
    public required int HighestEntityId { get; set; }
    public Type ComponentType => typeof(T);
    public required T[] Components { get; init;}
    public void AddEntity (World.Entity entity, T some)
    {
        if (Entities[entity.Id].Version > 0)
            throw new InvalidOperationException(
                $"Entity already has component {typeof(T)}");

        if (entity.Id >= MaxValue)
            throw new InvalidOperationException("Max stores reached.");

        Entities[entity.Id] = entity;
        Components[entity.Id] = some;

        if (HighestEntityId < entity.Id) HighestEntityId = entity.Id;
    }
}

public interface IComponent;
public class World
{
    private World () {}
    public required int MaxValue;
    public required Entity[] Entities;
    private int _entitiesCount = 0;
    internal IStore[] Stores = [];
    public int StoresCount { get; private set; } = 0;
    public readonly record struct Entity(int Id, int Version);
    public Entity CreateEntity()
    {
        if (_entitiesCount >= MaxValue)
            throw new InvalidOperationException("Max entities reached.");

        var e = new Entity(_entitiesCount, 1);
        Entities[_entitiesCount] = e;
        _entitiesCount++;
        return e;
    }

    public World AddStore<T> () where T : struct, IComponent
    {
        if (StoresCount >= MaxValue)
            throw new InvalidOperationException("Max stores reached.");

        var e = new Store<T>()
        {
            MaxValue = MaxValue,
            Components = new T[MaxValue],
            Entities = new Entity[MaxValue],
            HighestEntityId = 0
        };
        Stores[StoresCount++] = e;
        return this;
    }

    public Span<T> GetStore<T>()where T : struct, IComponent
    {
        for (int i = 0; i < StoresCount; i++)
        {
            var store = Stores[i];

            if (store.ComponentType == typeof(T))
                return ((Store<T>)store).Components;
        }

        throw new InvalidOperationException(
            $"No store for {typeof(T)} was added into the world.");
    }

    public static World New(int MaxValue = 100_000_00)
    {
        var w = new World
        {
            MaxValue = MaxValue,
            Entities = new Entity[MaxValue],
            Stores = new IStore[MaxValue],
        };
        return w;
    }
}

public static class WorldImpl
{
    public static List<World.Entity> GetWith<T> (World world) where T : struct, IComponent
    {
        var output = new List<World.Entity>();
        output.EnsureCapacity(world.MaxValue/2);

        for (int i = 0; i < world.StoresCount; i++)
        {
            var store = world.Stores[i];
            if (store.ComponentType == typeof(T))
            {
                var typedStore = (Store<T>)store;

                for (int j = 0; j <= typedStore.HighestEntityId; j++)
                    if (typedStore.Entities[j].Version > 0)
                        output.Add(typedStore.Entities[j]);
                        
                return output;
            }
        }
        throw new InvalidOperationException($"No store for {typeof(T)} was added into the world.");
    }

    public static List<World.Entity> And<T>(this List<World.Entity> span, World world) where T : struct, IComponent
    {
        for (int i = 0; i < world.StoresCount; i++)
        {
            var store = world.Stores[i];

            if (store.ComponentType == typeof(T))
            {
                var typedStore = (Store<T>)store;

                for (int j = span.Count - 1; j >= 0; j--)
                {
                    var entity = span[j];

                    if (entity.Id > typedStore.HighestEntityId || typedStore.Entities[entity.Id].Version == 0)
                        span.RemoveSwapBack(j);
                }

                return span;
            }
        }

        throw new InvalidOperationException(
            $"No store for {typeof(T)} was added into the world.");
    }

    public static void RemoveSwapBack<T>(this List<T> list, int index)
    {
        int last = list.Count - 1;

        if (index != last)
            list[index] = list[last];

        list.RemoveAt(last);
    }

    public static World.Entity AddComponent <T> (this World.Entity entity, World world, T some = default) where T : struct, IComponent
    {
        for (int i = 0; i < world.StoresCount; i++)
        {
            var store = world.Stores[i];
            if (store.ComponentType == typeof(T))
            {
                ((Store<T>)store).AddEntity(entity, some);
                return entity;
            }
        } 
        throw new InvalidOperationException($"No store for {typeof(T)} was added into the world.");
    }

    public static ref T GetComponent<T>(this World.Entity entity, World world)
        where T : struct, IComponent
    {
        for (int i = 0; i < world.StoresCount; i++)
        {
            var store = world.Stores[i];

            if (store.ComponentType == typeof(T))
            {
                var typedStore = (Store<T>)store;

                if (entity.Id <= typedStore.HighestEntityId &&
                    typedStore.Entities[entity.Id] == entity)
                {
                    return ref typedStore.Components[entity.Id];
                }

                throw new InvalidOperationException(
                    $"Entity {entity} has no component of type {typeof(T)}");
            }
        }

        throw new InvalidOperationException(
            $"No store for {typeof(T)} was added into the world.");
    }
}