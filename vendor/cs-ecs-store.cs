namespace Cecs;
public interface IStore;
public interface IComponent;
public record Store<T>(World World) : IStore where T : struct, IComponent
{
    private readonly int[] _entityToIndex = [.. Enumerable.Repeat(-1, World.MaxValue)];
    public List<Entity> Entities { get; } = new List<Entity>(World.MaxValue);
    public T[] Components { get; } = new T[World.MaxValue];

    internal bool HasEntity(Entity entity) => entity.Id >= 0 &&
                                            entity.Id < _entityToIndex.Length &&
                                            _entityToIndex[entity.Id] != -1;

    public void AddEntity(Entity entity, T component)
    {
        if (HasEntity(entity))
            throw new InvalidOperationException("Cannot add the entity twice...");
            
        _entityToIndex[entity.Id] = Entities.Count;
        Components[entity.Id] = component;
        Entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {   
        int index = _entityToIndex[entity.Id];
        if (index == -1) return;

        int lastIndex = Entities.Count - 1;
        if (index < lastIndex)
        {
            var lastEntity = Entities[lastIndex];
            Entities[index] = lastEntity;
            _entityToIndex[lastEntity.Id] = index;
            Components[index] = Components[lastIndex];
        }

        Entities.RemoveAt(lastIndex);
        Components[lastIndex] = default;
        _entityToIndex[entity.Id] = -1;
    }

    public ref T GetComponent(Entity entity)
    {
        if (_entityToIndex[entity.Id] != -1)
            return ref Components[entity.Id];

        throw new InvalidOperationException(
            $"Entity {entity} has no component of type {typeof(T)}");
    }
}

public static class StoreImpl
{
    extension(Entity entity)
    {
        public Entity AddEntityWith<T>(Store<T> store, T component = default) where T : struct, IComponent
        {
            store.AddEntity(entity, component);
            return entity;
        }

        public Entity RemoveEntityWith<T>(Store<T> store) where T : struct, IComponent
        {
            store.RemoveEntity(entity);
            return entity;
        }
    }
}