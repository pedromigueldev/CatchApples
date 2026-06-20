using Raylib_cs;
using Cecs;
using Cecs.Components;

const int screenWidth = 480;
const int screenHeight = 800;
const float appleScale = 0.2f;

Raylib.InitWindow(screenWidth, screenHeight, "Things");

World world = World.New(new(screenWidth, screenHeight), 100_000)
    .AddStore<Phase>()
    .AddStore<Rendereable<Texture2D>>()
    .AddStore<Obtainable>();

Texture2D appleTexture = Raylib.LoadTexture("assets/C_Logo.png");
Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

var random = new Random();
var appleComponent = new Rendereable<Texture2D>(appleTexture, new(appleTexture.Width, appleTexture.Height));
float scaledWidth = appleTexture.Width * appleScale;
var getRandomApplePosition = () => new Vec2(random.Next(0, screenWidth - (int)scaledWidth), -appleTexture.Height * appleScale);

Raylib.SetTargetFPS(60);

var phases = world.GetStore<Phase>();
var textures = world.GetStore<Rendereable<Texture2D>>();
var obtainable = world.GetStore<Obtainable>();

for (int i = 0; i < 20000; i++)
{
    world.CreateEntity()
        .AddComponent<Obtainable>(world)
        .AddComponent(world, appleComponent);
}

var player = world.CreateEntity()
    .AddComponent<Phase>(world)
    .AddComponent<Rendereable<Texture2D>>(world, new(basketTexture, new(basketTexture.Width, basketTexture.Height)));

SetPlayer(player, world);

var getenrateApple = (List<World.Entity> apples) =>
{
    if (apples.Count <= 0) return;
    phases.AddEntity(apples[^1], new Phase()
    {
        Position = new (getRandomApplePosition())
    });
};

float timer = 0f;
while (!Raylib.WindowShouldClose())
{
    RecicleApples();
    
    var apples = WorldImpl.GetEntitiesWith(obtainable).And(phases);
    var renderables = WorldImpl.GetEntitiesWith(phases).And(textures);
    MoveApples(apples, phases);
    MovePlayer(player, phases);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    foreach (var entity in renderables)
    {
        var pos = phases.GetComponent(entity);
        var tex = textures.GetComponent(entity);
        float scale = entity.Id == player.Id ? 1f : appleScale;
        DrawTextureScaled(tex.Texture2D, pos.Position.Point, scale);
    }

    Raylib.DrawFPS(0, 0);
    Raylib.EndDrawing();
}

void SetPlayer(World.Entity entity, World world)
{
    ref var pos = ref world.GetStore<Phase>().GetComponent(entity);
    ref var tex = ref world.GetStore<Rendereable<Texture2D>>().GetComponent(entity);

    var center = tex.Size.X / 4;
    var posX = (world.defaultSize.X / 2) - center;
    pos.Position = pos.Position with
    {
        Point = new(posX, 0)
    };
}

void MovePlayer(World.Entity entity, Store<Phase> positions)
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
        if(store.HasEntity(apples[i]))
        {
            ref var item = ref store.GetComponent(apples[i]);
            item = item with
            {
                Position = new Position(item.Position.Point + new Vec2(0, 5))
            };
        }
    }
}

void RecicleApples()
{
    var apples = WorldImpl.GetEntitiesWith(obtainable).And(phases);
    var applesN = WorldImpl.GetEntitiesWith(obtainable).AndNo(phases);

    for (int i = 0; i < apples.Count; i++)
    {
        ref var item = ref phases.GetComponent(apples[i]);
        if (item.Position.Point.Y > world.defaultSize.Y)
        {
            phases.RemoveEntity(apples[i]);
        }
    }

    timer += Raylib.GetFrameTime();
    if (timer >= 0.2f)
    {
        timer -= 0.2f;
        getenrateApple(applesN);
    }
}


void DrawTextureScaled(Texture2D texture, Vec2 position, float scale)
{
    var source = new Rectangle(0, 0, texture.Width, texture.Height);
    var dest = new Rectangle(position.X, position.Y, texture.Width * scale, texture.Height * scale);
    Raylib.DrawTexturePro(texture, source, dest, new(0, 0), 0f, Color.White);
}

public record struct Obtainable() : IComponent;

public record Message();
public record Collision(World.Entity Entity) : Message;
public record Obtain(World.Entity From, World.Entity To) : Message;
