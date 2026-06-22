#!/usr/bin/dotnet run

#:property OutputPath=./output
#:property TargetFramework=net11.0
#:property AppendTargetFrameworkToOutputPath=false
//#:property Optimize=true // removes debug info
#:property OptimizationPreference=Speed
#:property IlcOptimizationPreference=Speed
#:property WarningsAsErrors=nullable
#:property ImplicitUsings=enable
#:package Raylib-cs@8.0.0

#:include vendor/**/*.cs
#:include AppleSystem.cs
#:include build.cs

using CatchApple;
using Cecs;
using Cecs.Systems;
using Raylib_cs;

Build.BuildUtils.CopyDirectory("assets", Path.Combine("output", "assets"));
const int screenWidth = 480;
const int screenHeight = 800;

Raylib.InitWindow(screenWidth, screenHeight, "Things");

World world = World.New(new(screenWidth, screenHeight), 100_000)
    .AddStore<Geometry>()
    .AddStore<Velocity>()
    .AddStore<Position>()
    .AddStore<Rendereable<Texture2D>>()
    .AddStore<Obtainable>()
    .AddStore<Player>()
    .AddStore<Hitbox>()
    ;


Texture2D appleTexture = Raylib.LoadTexture("assets/apple.png");
Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

var appleComponent = new Rendereable<Texture2D>(
    appleTexture, 
    new(appleTexture.Width, appleTexture.Height),
    1
);

Raylib.SetTargetFPS(60);

var geometryStore = world.GetStore<Geometry>();
var velocityStore = world.GetStore<Velocity>();
var positionStore = world.GetStore<Position>();
var textureStore = world.GetStore<Rendereable<Texture2D>>();
var obtainableStore = world.GetStore<Obtainable>();
var hitBoxStore = world.GetStore<Hitbox>();

for (int i = 0; i < 30 - 1; i++)
{
    world.CreateEntity()
        .AddComponent<Obtainable>(world)
        .AddComponent(world, appleComponent)
        .AddComponent<Hitbox>(world)
        .AddComponent<Geometry>(world, new () { Point = new(appleTexture.Width, appleTexture.Height) })
        .AddComponent<Velocity>(world, new () { Point = new ( 0, 2) })
        ;
}

float appleSpeed = 5;
float tick = 1f;
float timer = 0f;

var center = basketTexture.Width / 2;
var posX = (world.defaultSize.X / 2) - center;
var player = world.CreateEntity()
            .AddComponent<Player>(world)
            .AddComponent<Geometry>(world, new () { Point = new (basketTexture.Width, basketTexture.Height) })
            .AddComponent<Position>(world, new ()
            {
                Point = new (posX, world.defaultSize.Y - basketTexture.Height - 20)
            })
            .AddComponent<Velocity>(world, new ())
            .AddComponent<Rendereable<Texture2D>>(world, new(basketTexture, new(basketTexture.Width, basketTexture.Height), 1))
            .AddComponent<Hitbox>(world);
            

var buffer = new List<World.Entity> (world.MaxValue);
var searchArea = new List<World.Entity> (world.MaxValue);
Dictionary<World.Entity, List<World.Entity>> hitPair = new (world.MaxValue);
for (int i = 0; i < world.MaxValue; i++)
    hitPair.Add(new World.Entity(i, 1), []);

int coughtApples = 0;
int lostApples = 0;
while (!Raylib.WindowShouldClose())
{
    PlayerSystem.PlayerMove plyerMovement = PlayerSystem.PlayerMove.None;
    if (Raylib.IsKeyDown(KeyboardKey.A)) plyerMovement |= PlayerSystem.PlayerMove.Left;
    if (Raylib.IsKeyDown(KeyboardKey.D)) plyerMovement |= PlayerSystem.PlayerMove.Right;
    PlayerSystem.Move(player, velocityStore, plyerMovement, 10);
    
    lostApples += AppleSystem.RevictApples(positionStore, obtainableStore, buffer, (int)world.defaultSize.Y);

    AppleSystem.RecicleApples(
        buffer, 
        positionStore,
        velocityStore,
        geometryStore, 
        obtainableStore, 
        world,
        tick,
        ref timer,
        ref appleSpeed
    );
    
    GeometrySystem.Move(positionStore, velocityStore, buffer);
    HitBoxSystem.ScanCollisions(hitBoxStore, positionStore, geometryStore, hitPair, buffer, searchArea);

    var items = hitPair[player];
    foreach (var item in items)
    {
        coughtApples++;
        positionStore.RemoveEntity(item);
        if (coughtApples % 10 == 0)
        {
            appleSpeed += 0.2f;
            tick -= 0.05f;
        }
    }
    hitPair[player].Clear();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    RenderSystem.RenderEntities (textureStore, positionStore, DrawTextureScaled, buffer);

    Raylib.DrawFPS(0, 0);
    Raylib.DrawText($"APPLES COUGHT {coughtApples}", 10, 30, 24, Color.Red);
    Raylib.DrawText($"APPLES LOST {lostApples}", 10, 50, 24, Color.Red);
    Raylib.EndDrawing();
}

static void DrawTextureScaled(Rendereable<Texture2D> rendereable, Position position)
{
    var source = new Rectangle(0, 0, rendereable.Texture2D.Width, rendereable.Texture2D.Height);
    var dest = new Rectangle(position.Point.X, position.Point.Y, rendereable.Texture2D.Width * rendereable.Scale, rendereable.Texture2D.Height * rendereable.Scale);
    Raylib.DrawTexturePro(rendereable.Texture2D, source, dest, new(0, 0), 0f, Color.White);
}

public record struct Obtainable() : IComponent;