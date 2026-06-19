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
    var Random = new Random();
    Texture2D logoTexture = Raylib.LoadTexture("assets/C_Logo.png");
    var apple = new Rendereable(logoTexture, new (logoTexture.Width, logoTexture.Height));
    
    float scale = 0.5f;
    float scaledWidth = apple.size.X * scale;
    float scaledHeight = apple.size.Y * scale;
    
    for (int i = 0; i < 10000; i++)
    {
        float x = Random.Next(0, screenWidth - (int)scaledWidth);
        float y = -scaledHeight;
        var position = new Vec2(x, y);

        world.CreateEntity()
            .AddComponent<Position>(world, new (position))
            .AddComponent<Velocity>(world)
            .AddComponent(world, apple)
            .AddComponent<Obtainable>(world);
    }
    
    WorldImpl.GetWith<Obtainable>(world, Apples);
};

Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");
var player = world
    .CreateEntity()
    .AddComponent<Position>(world)
    .AddComponent<Velocity>(world)
    .AddComponent(world, new Rendereable(basketTexture, new (basketTexture.Width, basketTexture.Height)));
    
RegisterApples(world);
Raylib.SetTargetFPS(60);

var items = new List<World.Entity>(world.MaxValue);
var shouldReload = true;
while (!Raylib.WindowShouldClose())
{
    if (shouldReload)
    {
        WorldImpl.GetWith<Rendereable>(world, items).And<Position>(world);
        shouldReload = false;
    }
    var positions = world.GetStore<Position>();
    var textures = world.GetStore<Rendereable>();
    PositionSystem.MovePlayerWASD(player, world);
    PositionSystem.MoveApplesDown(Apples, positions);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    RendereableSystem.DrawAllTexturesOf(positions, textures, items);

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

    public static void MoveApplesDown (List<World.Entity> entity, Span<Position> store)
    {
        for (int i = 0; i < entity.Count; i++)
            store[entity[i].Id].Point += new Vec2(0, 1);
    }
}

public static class RendereableSystem
{
    public static void DrawTextureScaled(Texture2D texture, Vec2 position, float scale, Color color)
    {
        var source = new Rectangle(0, 0, texture.Width, texture.Height);
        var dest = new Rectangle(
            position.X, 
            position.Y, 
            texture.Width * scale, 
            texture.Height * scale
        );
        var origin = new Vec2(0, 0);
        
        Raylib.DrawTexturePro(texture, source, dest, new (origin.X, origin.Y), 0.0f, color);
    }

    public static void DrawAllTexturesOf (Span<Position> positions, Span<Rendereable> textures, List<World.Entity> items)
    {
        foreach (var item in items)
        {
            DrawTextureScaled(textures[item.Id].Texture2D, positions[item.Id].Point, 0.5f, Color.White);
        }
    }
}