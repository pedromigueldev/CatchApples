namespace Cecs;
internal interface IStore
{
    int MaxValue { get; }
    List<World.Entity> Entities { get; }
    Type ComponentType { get; }
}

internal class Store<T> : IStore where T : struct, IComponent {
    public int MaxValue { get; }
    public List<World.Entity> Entities { get; }
    public Type ComponentType => typeof(T);
    public T[] Components { get; }
    private int[] _entityToIndex;
    
    public Store(int maxValue)
    {
        MaxValue = maxValue;
        Components = new T[maxValue];
        Entities = new List<World.Entity>(maxValue);
        _entityToIndex = new int[maxValue];
        Array.Fill(_entityToIndex, -1);
    }
    
    public void AddEntity (World.Entity entity, T some)
    {
        if (entity.Id >= MaxValue)
            throw new InvalidOperationException("Max stores reached.");

        if (_entityToIndex[entity.Id] != -1)
            throw new InvalidOperationException(
                $"Entity already has component {typeof(T)}");

        _entityToIndex[entity.Id] = Entities.Count;
        Entities.Add(entity);
        Components[entity.Id] = some;
    }
    
    public void RemoveEntity(World.Entity entity)
    {
        int index = _entityToIndex[entity.Id];
        if (index == -1) return;
        
        int last = Entities.Count - 1;
        if (index != last)
        {
            Entities[index] = Entities[last];
            _entityToIndex[Entities[index].Id] = index;
        }
        Entities.RemoveAt(last);
        _entityToIndex[entity.Id] = -1;
    }
    
    public bool HasEntity(World.Entity entity)
    {
        return entity.Id < MaxValue && _entityToIndex[entity.Id] != -1;
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

        Stores[StoresCount++] = new Store<T>(MaxValue);
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

    public static World New(int MaxValue = 10000)
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
    public static List<World.Entity> GetWith<T> (World world, List<World.Entity> output) where T : struct, IComponent
    {
        output.Clear();
        for (int i = 0; i < world.StoresCount; i++)
        {
            var store = world.Stores[i];
            if (store.ComponentType == typeof(T))
            {
                var typedStore = (Store<T>)store;

                for (int j = 0; j < typedStore.Entities.Count; j++)
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
                    if (!typedStore.HasEntity(span[j]))
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

                if (typedStore.HasEntity(entity))
                    return ref typedStore.Components[entity.Id];

                throw new InvalidOperationException(
                    $"Entity {entity} has no component of type {typeof(T)}");
            }
        }

        throw new InvalidOperationException(
            $"No store for {typeof(T)} was added into the world.");
    }
}