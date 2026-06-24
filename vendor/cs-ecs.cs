using System.Runtime.CompilerServices;
using Cecs.Systems;
namespace Cecs;
public readonly record struct Entity(int Id, int Version);
public record World (Vec2 DefaultSize, int MaxValue = 10000)
{
    internal readonly List<Entity> Entities = new (MaxValue);
    internal List<IStore> Stores = new (15);
    internal List<IArchetpe> Archetypes = new (15);
}
public static class WorldImpl
{
    extension (World self)
    {
        public Entity CreateEntity()
        {
            if (self.Entities.Count + 1 >= self.MaxValue)
                throw new InvalidOperationException("Max entities reached.");

            var e = new Entity(self.Entities.Count, 1);
            self.Entities.Add(e);
            return e;
        }

        public World AddStore<T> () where T : struct, IComponent
        {
            if (self.Stores.Count >= self.MaxValue)
                throw new InvalidOperationException("Max stores reached.");

            var val = new Store<T>(self);
            self.Stores.Add(val);
            return self;
        }

        public Store<T> GetStore<T>() where T : struct, IComponent
        {
            for (int i = 0; i < self.Stores.Count; i++)
            {
                var store = self.Stores[i];

                if (store.GetType() == typeof(Store<T>))
                    return (Store<T>)self.Stores[i];
            }

            throw new InvalidOperationException(
                $"No store for {typeof(T)} was added into the world.");
        }

        public World AddArchetype<T> () where T : struct, ITuple
        {
            if (self.Archetypes.Count >= self.MaxValue)
                throw new InvalidOperationException("Max stores reached.");

            var val = new Archetype<T>(self);
            self.Archetypes.Add(val);
            return self;
        }

        public Archetype<T> GetArchetype<T>() where T : struct, ITuple
        {
            for (int i = 0; i < self.Archetypes.Count; i++)
            {
                var store = self.Archetypes[i];

                if (store.GetType() == typeof(Archetype<T>))
                    return (Archetype<T>)self.Archetypes[i];
            }

            throw new InvalidOperationException(
                $"No archetype for {typeof(T)} was added into the world.");
        }
    }
}

public static class Ext {
    public static List<Entity> GetEntitiesWith<T> (this List<Entity> entities, Store<T> store) where T : struct, IComponent
    {
        entities.Clear();
        entities.EnsureCapacity(store.Entities.Count);
        entities.AddRange(store.Entities);
        return entities;
    }

    public static List<Entity> WithNo<T>(this List<Entity> buffer, Store<T> store) where T : struct, IComponent
    {
        buffer.Clear();
        buffer.EnsureCapacity(store.Entities.Count);
        buffer.AddRange(store.Entities);
        
        for (int j = buffer.Count - 1; j >= 0; j--)
        {
            if (store.HasEntity(buffer[j]))
                buffer.RemoveSwapBack(j);
        }

        return buffer;
    }

    public static List<Entity> With<T>(this List<Entity> buffer, Store<T> store) where T : struct, IComponent
    {
        buffer.Clear();
        buffer.EnsureCapacity(store.Entities.Count);
        buffer.AddRange(store.Entities);
        
        for (int j = buffer.Count - 1; j >= 0; j--)
        {
            if (!store.HasEntity(buffer[j]))
                buffer.RemoveSwapBack(j);
        }

        return buffer;
    }

    public static List<Entity> And<T>(this List<Entity> span, Store<T> store) where T : struct, IComponent
    {
        for (int j = span.Count - 1; j >= 0; j--)
        {
            if (!store.HasEntity(span[j]))
                span.RemoveSwapBack(j);
        }

        return span;
    }

    public static List<Entity> AndNo<T>(this List<Entity> span, Store<T> store) where T : struct, IComponent
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
}