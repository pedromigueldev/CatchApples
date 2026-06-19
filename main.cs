using Raylib_cs;
using Cecs;


const int screenWidth = 480;
const int screenHeight = 800;
Raylib.InitWindow(screenWidth, screenHeight, "Things");

World world = World.New()
    .AddStore<Position>()
    .AddStore<Velocity>()
    .AddStore<Rendereable>()
    .AddStore<Obtainable>();

List<World.Entity> Apples = [];
var RegisterApples = (World world) =>
{
    Texture2D logoTexture = Raylib.LoadTexture("assets/C_Logo.png");
    var apple = new Rendereable(logoTexture, new (logoTexture.Width, logoTexture.Height));
    for (int i = 0; i < 12; i++)
    {
        world.CreateEntity()
            .AddComponent<Velocity>(world)
            .AddComponent(world, apple)
            .AddComponent<Obtainable>(world);
    }
    
    Apples = WorldImpl.GetWith<Obtainable>(world);
};

Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");
var player = world
    .CreateEntity()
    .AddComponent<Position>(world)
    .AddComponent<Velocity>(world)
    .AddComponent(world, new Rendereable(basketTexture, new (basketTexture.Width, basketTexture.Height)));
    
RegisterApples(world);
Raylib.SetTargetFPS(60);

ref var PlayerTexture =  ref WorldImpl.GetComponent<Rendereable>(player, world);
ref var PlayerPosition = ref WorldImpl.GetComponent<Position>(player, world);

while (!Raylib.WindowShouldClose())
{
    PositionSystem.MovePlayerWASD(player, world);
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);
    
    Raylib.DrawTexture(PlayerTexture.Texture2D, (int)PlayerPosition.Point.X, (int)PlayerPosition.Point.Y, Color.White);

    Raylib.DrawFPS(0, 0);
    Raylib.EndDrawing();
}

public readonly record struct Vec2 (float X, float Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) => new (a.X + b.X, a.Y + b.Y);
    public static Vec2 operator *(Vec2 a, Vec2 b) => new (a.X * b.X, a.Y * b.Y);
};
public record struct Position(Vec2 Point) : Cecs.IComponent;
public record struct Obtainable() : Cecs.IComponent;
public record struct Velocity(Vec2 Point) : Cecs.IComponent;
public record struct Rendereable(Texture2D Texture2D, Vec2 size) : Cecs.IComponent;

public static class PositionSystem
{
    public static void MovePlayerWASD (World.Entity entity, World world)
    {
        ref var player = ref WorldImpl.GetComponent<Position>(entity, world);
        if (Raylib.IsKeyDown(KeyboardKey.W)) player.Point += new Vec2(0, -1);
        if (Raylib.IsKeyDown(KeyboardKey.A)) player.Point += new Vec2(-1, 0);
        if (Raylib.IsKeyDown(KeyboardKey.S)) player.Point += new Vec2(0, 1);
        if (Raylib.IsKeyDown(KeyboardKey.D)) player.Point += new Vec2(1, 0);
    }
}