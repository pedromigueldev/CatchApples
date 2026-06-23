namespace Cecs;

public static class WorldQueryImpl
{
    public static (Store<T1>, Store<T2>) GetArchetypeOf<T1, T2>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>) GetArchetypeOf<T1, T2, T3>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>) GetArchetypeOf<T1, T2, T3, T4>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>) GetArchetypeOf<T1, T2, T3, T4, T5>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>(), world.GetStore<T5>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>) GetArchetypeOf<T1, T2, T3, T4, T5, T6>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>(), world.GetStore<T5>(), world.GetStore<T6>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>, Store<T7>) GetArchetypeOf<T1, T2, T3, T4, T5, T6, T7>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>(), world.GetStore<T5>(), world.GetStore<T6>(), world.GetStore<T7>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>, Store<T7>, Store<T8>) GetArchetypeOf<T1, T2, T3, T4, T5, T6, T7, T8>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent
        where T8 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>(), world.GetStore<T5>(), world.GetStore<T6>(), world.GetStore<T7>(), world.GetStore<T8>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>, Store<T7>, Store<T8>, Store<T9>) GetArchetypeOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent
        where T8 : struct, IComponent
        where T9 : struct, IComponent
    {
        return (world.GetStore<T1>(), world.GetStore<T2>(), world.GetStore<T3>(), world.GetStore<T4>(), world.GetStore<T5>(), world.GetStore<T6>(), world.GetStore<T7>(), world.GetStore<T8>(), world.GetStore<T9>());
    }

    public static (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>, Store<T7>, Store<T8>, Store<T9>, Store<T10>) GetArchetypeOf<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this World world)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent
        where T8 : struct, IComponent
        where T9 : struct, IComponent
        where T10 : struct, IComponent
    {
        return (
            world.GetStore<T1>(),
            world.GetStore<T2>(),
            world.GetStore<T3>(),
            world.GetStore<T4>(),
            world.GetStore<T5>(),
            world.GetStore<T6>(),
            world.GetStore<T7>(),
            world.GetStore<T8>(),
            world.GetStore<T9>(),
            world.GetStore<T10>());
    }
}

public static class WorldQueryAddRemoveImpl
{
    public static void AddEntity<T1, T2>(
        this (Store<T1>, Store<T2>) archetype,
        World.Entity entity,
        T1 d1, T2 d2)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var (s1, s2) = archetype;
        s1.AddEntity(entity, d1);
        s2.AddEntity(entity, d2);
    }

    public static void RemoveEntity<T1, T2>(
        this (Store<T1>, Store<T2>) archetype,
        World.Entity entity)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var (s1, s2) = archetype;
        s1.RemoveEntity(entity);
        s2.RemoveEntity(entity);
    }

    public static void AddEntity<T1, T2, T3>(
        this (Store<T1>, Store<T2>, Store<T3>) archetype,
        World.Entity entity,
        T1 d1, T2 d2, T3 d3)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var (s1, s2, s3) = archetype;
        s1.AddEntity(entity, d1);
        s2.AddEntity(entity, d2);
        s3.AddEntity(entity, d3);
    }

    public static void RemoveEntity<T1, T2, T3>(
        this (Store<T1>, Store<T2>, Store<T3>) archetype,
        World.Entity entity)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var (s1, s2, s3) = archetype;
        s1.RemoveEntity(entity);
        s2.RemoveEntity(entity);
        s3.RemoveEntity(entity);
    }

    public static void AddEntity<T1, T2, T3, T4>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>) archetype,
        World.Entity entity,
        T1 d1, T2 d2, T3 d3, T4 d4)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        var (s1, s2, s3, s4) = archetype;
        s1.AddEntity(entity, d1);
        s2.AddEntity(entity, d2);
        s3.AddEntity(entity, d3);
        s4.AddEntity(entity, d4);
    }

    public static void RemoveEntity<T1, T2, T3, T4>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>) archetype,
        World.Entity entity)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        var (s1, s2, s3, s4) = archetype;
        s1.RemoveEntity(entity);
        s2.RemoveEntity(entity);
        s3.RemoveEntity(entity);
        s4.RemoveEntity(entity);
    }

    public static void AddEntity<T1, T2, T3, T4, T5>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>) archetype,
        World.Entity entity, 
        T1 d1, T2 d2, T3 d3, T4 d4, T5 d5)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        var (s1, s2, s3, s4, s5) = archetype;
        s1.AddEntity(entity, d1);
        s2.AddEntity(entity, d2);
        s3.AddEntity(entity, d3);
        s4.AddEntity(entity, d4);
        s5.AddEntity(entity, d5);
    }

    public static void RemoveEntity<T1, T2, T3, T4, T5>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>) archetype,
        World.Entity entity)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        var (s1, s2, s3, s4, s5) = archetype;
        s1.RemoveEntity(entity);
        s2.RemoveEntity(entity);
        s3.RemoveEntity(entity);
        s4.RemoveEntity(entity);
        s5.RemoveEntity(entity);
    }

    public static void AddEntity<T1, T2, T3, T4, T5, T6>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>) archetype,
        World.Entity entity,
        T1 d1 = default, T2 d2= default, T3 d3= default, T4 d4= default, T5 d5= default, T6 d6= default)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        var (s1, s2, s3, s4, s5, s6) = archetype;
        s1.AddEntity(entity, d1);
        s2.AddEntity(entity, d2);
        s3.AddEntity(entity, d3);
        s4.AddEntity(entity, d4);
        s5.AddEntity(entity, d5);
        s6.AddEntity(entity, d6);
    }

    public static void RemoveEntity<T1, T2, T3, T4, T5, T6>(
        this (Store<T1>, Store<T2>, Store<T3>, Store<T4>, Store<T5>, Store<T6>) archetype,
        World.Entity entity)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        var (s1, s2, s3, s4, s5, s6) = archetype;
        s1.RemoveEntity(entity);
        s2.RemoveEntity(entity);
        s3.RemoveEntity(entity);
        s4.RemoveEntity(entity);
        s5.RemoveEntity(entity);
        s6.RemoveEntity(entity);
    }
}