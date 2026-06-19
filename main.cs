using Raylib_cs;
using Cecs;
using Cecs.Components;

const int screenWidth = 480;
const int screenHeight = 800;
const float appleScale = 0.2f;

Raylib.InitWindow(screenWidth, screenHeight, "Things");

World world = World.New(new (400, 400), 100_000)
    .AddStore<Phase>()
    .AddStore<Rendereable<Texture2D>>()
    .AddStore<Obtainable>();

Texture2D appleTexture = Raylib.LoadTexture("assets/C_Logo.png");
Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

var random = new Random();
var appleComponent = new Rendereable<Texture2D>(appleTexture, new(appleTexture.Width, appleTexture.Height));
float scaledWidth = appleTexture.Width * appleScale;

for (int i = 0; i < 10_000; i++)
{
    var applePhase = new Phase()
    {
        Position = new Position(new(random.Next(0, screenWidth - (int)scaledWidth), -appleTexture.Height * appleScale))
    };

    world.CreateEntity()
    .AddComponent<Obtainable>(world)
    .AddComponent(world, applePhase)
    .AddComponent(world, appleComponent);
}
var player = world.CreateEntity()
    .AddComponent<Phase>(world)
    .AddComponent<Rendereable<Texture2D>>(world, new (basketTexture, new(basketTexture.Width, basketTexture.Height)));
var apples = WorldImpl.GetEntitiesWith(world.GetStore<Obtainable>());

Raylib.SetTargetFPS(60);

var phases = world.GetStore<Phase>();
var textures = world.GetStore<Rendereable<Texture2D>>();
var allRenderables = WorldImpl.GetEntitiesWith(phases).And(textures);
SetPlayer(player, world);
while (!Raylib.WindowShouldClose())
{
    MovePlayer(player, phases);
    MoveApples(apples, phases);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);
    
    foreach (var entity in allRenderables)
    {
        var pos = phases.GetComponent(entity);
        var tex = textures.GetComponent(entity);
        float scale = entity.Id == player.Id ? 1f : appleScale;
        DrawTextureScaled(tex.Texture2D, pos.Position.Point, scale);
    }
    
    Raylib.DrawFPS(0, 0);
    Raylib.EndDrawing();
}

void SetPlayer (World.Entity entity, World world)
{
    ref var pos = ref world.GetStore<Phase>().GetComponent(entity);
    ref var tex = ref world.GetStore<Rendereable<Texture2D>>().GetComponent(entity);

    var center = tex.Size.X / 4;
    var posX = (world.defaultSize.X / 2) - center;
    pos.Position = pos.Position with
    {
        Point = new (posX, 0)
    };
}

void MovePlayer (World.Entity entity, Store<Phase> positions)
{
    ref var pos = ref positions.GetComponent(entity);
    var point = pos.Position.Point;
    if (Raylib.IsKeyDown(KeyboardKey.A)) point += new Vec2(-1, 0);
    if (Raylib.IsKeyDown(KeyboardKey.D)) point += new Vec2(1, 0);
    pos = new Phase(new Position(point), pos.Velocity); 
}

void MoveApples(List<World.Entity> apples, Store<Phase> store)
{
    for (int i = 0; i < apples.Count; i++)
    {
        ref var item = ref store.GetComponent(apples[i]);
        item = item with {
            Position = new Position(item.Position.Point + new Vec2(0, 1))
        };
    }
}

void DrawTextureScaled(Texture2D texture, Vec2 position, float scale)
{
    var source = new Rectangle(0, 0, texture.Width, texture.Height);
    var dest = new Rectangle(position.X, position.Y, texture.Width * scale, texture.Height * scale);
    Raylib.DrawTexturePro(texture, source, dest, new (0,0), 0f, Color.White);
}

public record struct Obtainable() : IComponent;