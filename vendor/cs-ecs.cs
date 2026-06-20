namespace Cecs;

internal interface IStore
{
    Type ComponentType { get; }
};
public record Store<T> : IStore where T : struct, IComponent {
    internal readonly int[] _entityToIndex;
    public List<World.Entity> Entities { get; }
    public T[] Components { get; }
    public Type ComponentType => typeof(T);
    public Store(int maxValue)
    {
        Components = new T[maxValue];
        Entities = new List<World.Entity>(maxValue);
        _entityToIndex = new int[maxValue];
        Array.Fill(_entityToIndex, -1);
    }
}

public interface IComponent;
public class World
{
    private World () {}
    public int MaxValue { get; init; }
    private Entity[] Entities = [];
    internal List<IStore> Stores = [];
    private int _entitiesCount = 0;
    public required Components.Vec2 defaultSize;
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
        if (Stores.Count >= MaxValue)
            throw new InvalidOperationException("Max stores reached.");

        var val = new Store<T>(MaxValue);
        Stores.Add(val);

        return this;
    }

    public Store<T> GetStore<T>()where T : struct, IComponent
    {
        for (int i = 0; i < Stores.Count; i++)
        {
            var store = Stores[i];

            if (store.ComponentType == typeof(T))
                return (Store<T>)Stores[i];
        }

        throw new InvalidOperationException(
            $"No store for {typeof(T)} was added into the world.");
    }

    public static World New(Components.Vec2 defaultSize, int MaxValue = 10000)
    {
        var w = new World
        {
            defaultSize = defaultSize,
            MaxValue = MaxValue,
            Entities = new Entity[MaxValue],
            Stores = new (15),
        };
        return w;
    }
}

public static class WorldImpl
{
    public static List<World.Entity> GetEntitiesWith<T> (this List<World.Entity> entities, Store<T> store) where T : struct, IComponent
    {
        entities.Clear();
        entities.EnsureCapacity(store.Entities.Capacity);
        entities.AddRange(store.Entities);
        return entities;
    }

    public static List<World.Entity> And<T>(this List<World.Entity> span, Store<T> store) where T : struct, IComponent
    {
        for (int j = span.Count - 1; j >= 0; j--)
        {
            if (!store.HasEntity(span[j]))
                span.RemoveSwapBack(j);
        }

        return span;
    }

    public static List<World.Entity> AndNo<T>(this List<World.Entity> span, Store<T> store) where T : struct, IComponent
    {
        for (int j = span.Count - 1; j >= 0; j--)
        {
            if (store.HasEntity(span[j]))
                span.RemoveSwapBack(j);
        }

        return span;
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
        for (int i = 0; i < world.Stores.Count; i++)
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
}

public static class StoreImpl
{
    public static void AddEntity <T>(this Store<T> store, World.Entity entity, T some)  where T : struct, IComponent
    {
        if (entity.Id >= store.Entities.Capacity)
            throw new InvalidOperationException("Max stores reached.");

        if (store._entityToIndex[entity.Id] != -1)
            throw new InvalidOperationException(
                $"Entity already has component {typeof(T)}");

        store._entityToIndex[entity.Id] = store.Entities.Count;
        store.Entities.Add(entity);
        store.Components[entity.Id] = some;
    }
    
    public static void RemoveEntity <T>(this Store<T> store, World.Entity entity) where T : struct, IComponent
    {
        int index = store._entityToIndex[entity.Id];
        if (index == -1) return;
        
        int last = store.Entities.Count - 1;
        if (index <= last)
        {
            store.Entities[index] = store.Entities[last];
            store._entityToIndex[store.Entities[index].Id] = index;
        }
        store.Entities.RemoveAt(last);
        store._entityToIndex[entity.Id] = -1;
    }
    
    public static bool HasEntity <T>(this Store<T> store, World.Entity entity) where T : struct, IComponent
    {
        return entity.Id < store.Entities.Capacity && store._entityToIndex[entity.Id] != -1;
    }

    public static ref T GetComponent<T>(this Store<T> store, World.Entity entity) where T : struct, IComponent
    {
        if (store.HasEntity(entity))
            return ref store.Components[entity.Id];

        throw new InvalidOperationException(
            $"Entity {entity} has no component of type {typeof(T)}");
    }
}