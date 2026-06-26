
namespace Cecs;

public interface IComponentGroup;
public class ComponentGroup<t1, t2> : IComponentGroup
where t1 : struct, IComponent
where t2 : struct, IComponent
{
    private readonly World world;
    public List<Entity> entities = [];
    public List<t1> T1 = [];
    public List<t2> T2 = [];

    public ComponentGroup (World world)
    {
        this.world = world;
        world.AddStore<t1>();
        world.AddStore<t2>();
    }

    public ComponentGroup<t1, t2> AddEntity(Entity entity, t1 t1, t2 t2)
    {
        entities.Add(entity);
        world.GetStore<t1>().AddEntity(entity, t1);
        world.GetStore<t2>().AddEntity(entity, t2);
        T1.Add(t1);
        T2.Add(t2);
        return this;
    }

    public void RemoveEntity(Entity entity)
    {   
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == entity)
            {
                entities.RemoveSwapBack(i);
                world.GetStore<t1>().RemoveEntity(entity);
                world.GetStore<t2>().RemoveEntity(entity);
                T1.RemoveSwapBack(i);
                T2.RemoveSwapBack(i);
            }
        }
    }
}

public static class CGImpl
{
    extension (Entity entity)
    {
        public Entity AddEntityWith<t1, t2>(ComponentGroup<t1, t2> with, t1 T1, t2 T2)
            where t1 : struct, IComponent
            where t2 : struct, IComponent
        {
            with.AddEntity(entity, T1, T2);
            return entity;
        }
    }

    extension (World self)
    {
        public World AddComponentGroup<t1, t2>()
            where t1 : struct, IComponent
            where t2 : struct, IComponent
        {
            if (self.Archetypes.Count >= self.MaxValue)
                throw new InvalidOperationException("Max stores reached.");

            var val = new ComponentGroup<t1, t2>(self);
            self.ComponentGroups.Add(val);
            return self;
        }

        public ComponentGroup<t1, t2> GetComponentGroup<t1, t2>()
            where t1 : struct, IComponent
            where t2 : struct, IComponent
        {
            for (int i = 0; i < self.ComponentGroups.Count; i++)
            {
                var store = self.ComponentGroups[i];

                if (store.GetType() == typeof(ComponentGroup<t1, t2>))
                    return (ComponentGroup<t1, t2>)self.ComponentGroups[i];
            }

            throw new InvalidOperationException(
                $"No archetype for {typeof(ComponentGroup<t1, t2>)} was added into the world.");
        }
    }
}