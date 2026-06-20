using Raylib_cs;
using Cecs;
using CatchApple;
using Cecs.Systems;

const int screenWidth = 480;
const int screenHeight = 800;

Raylib.InitWindow(screenWidth, screenHeight, "Things");

World world = World.New(new(screenWidth, screenHeight), 100_000)
    .AddStore<Geometry>()
    .AddStore<Rendereable<Texture2D>>()
    .AddStore<Obtainable>()
    .AddStore<Player>();

Texture2D appleTexture = Raylib.LoadTexture("assets/C_Logo.png");
Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

var appleComponent = new Rendereable<Texture2D>(appleTexture, new(appleTexture.Width, appleTexture.Height));

Raylib.SetTargetFPS(60);

var geometryStore = world.GetStore<Geometry>();
var textures = world.GetStore<Rendereable<Texture2D>>();
var obtainableStore = world.GetStore<Obtainable>();

for (int i = 0; i < 10 - 1; i++)
{
    world.CreateEntity()
        .AddComponent<Obtainable>(world)
        .AddComponent(world, appleComponent);
}

float tick = 0.2f;
float timer = 0f;
var player = PlayerSystem
            .New(world, new(basketTexture.Width, basketTexture.Height))
            .AddComponent<Rendereable<Texture2D>>(world, new(basketTexture, new(basketTexture.Width, basketTexture.Height)));
            
var buffer = new List<World.Entity> (world.MaxValue);

while (!Raylib.WindowShouldClose())
{

    PlayerSystem.PlayerMove plyerMovement = PlayerSystem.PlayerMove.Still;

    if (Raylib.IsKeyDown(KeyboardKey.W)) plyerMovement = PlayerSystem.PlayerMove.Up;
    if (Raylib.IsKeyDown(KeyboardKey.A)) plyerMovement = PlayerSystem.PlayerMove.Left;
    if (Raylib.IsKeyDown(KeyboardKey.S)) plyerMovement = PlayerSystem.PlayerMove.Down;
    if (Raylib.IsKeyDown(KeyboardKey.D)) plyerMovement = PlayerSystem.PlayerMove.Right;
    
    AppleSystem.RevictApples(buffer, geometryStore, obtainableStore, world);
    AppleSystem.RecicleApples(buffer, appleTexture, geometryStore, obtainableStore, world, tick, ref timer);

    GeometrySystem.Move(geometryStore);
    PlayerSystem.Move(player, geometryStore, plyerMovement);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    foreach (var entity in WorldImpl.GetEntitiesWith(buffer, geometryStore).And(textures))
    {
        var pos = geometryStore.GetComponent(entity);

        if (pos.Position.Point < world.defaultSize)
        {
            var tex = textures.GetComponent(entity);
            float scale = entity.Id == player.Id ? 1f : 0.2f;
            DrawTextureScaled(tex.Texture2D, pos.Position.Point, scale);
        }
    }

    Raylib.DrawFPS(0, 0);
    Raylib.EndDrawing();
}

static void DrawTextureScaled(Texture2D texture, Vec2 position, float scale)
{
    var source = new Rectangle(0, 0, texture.Width, texture.Height);
    var dest = new Rectangle(position.X, position.Y, texture.Width * scale, texture.Height * scale);
    Raylib.DrawTexturePro(texture, source, dest, new(0, 0), 0f, Color.White);
}

public record struct Obtainable() : IComponent;