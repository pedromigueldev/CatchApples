using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cecs;

internal interface IArchetpe
{
    bool HasEntity(Entity entity);
};

public record Archetype<CT> (World World) : IArchetpe where CT : struct, ITuple
{
    internal readonly List<Entity> Entities = new (World.MaxValue); 
    public List<CT> ComponentsTuple = new (World.MaxValue);

    public bool HasEntity(Entity entity)
    {
        for (int i = 0; i < Entities.Count; i++)
        {
            if (Entities[i] == entity) return true;   
        }

        return false;
    }

    public Archetype<CT> AddEntity(Entity entity, CT component)
    {
        foreach (var item in World.Archetypes)
            if (item.HasEntity(entity))
                throw new InvalidOperationException("Enity cannot have multiple archetypes...");

        Entities.Add(entity);
        ComponentsTuple.Add(component);
        return this;
    }
    public void RemoveEntity(Entity entity)
    {   
        for (int i = 0; i < Entities.Count; i++)
        {
            if (Entities[i] == entity)
            {
                Entities.RemoveSwapBack(i);
                ComponentsTuple.RemoveSwapBack(i);
            }
        }
    }

    public ref CT GetComponent(Entity entity)
    {
        for (int i = 0; i < Entities.Count; i++)
        {
            if (Entities[i] == entity)
            {
                return ref CollectionsMarshal.AsSpan(ComponentsTuple)[i];
            }
        }

        throw new InvalidOperationException(
            $"Entity {entity} has no component of type {typeof(Archetype<CT>)}");
    }
}

public static class WithImpl
{
    extension (Entity entity)
    {
        public Entity AddEntityWith<T>(Archetype<T> with, T component) where T : struct, ITuple
        {
            with.AddEntity(entity, component);
            return entity;
        }
    }
}