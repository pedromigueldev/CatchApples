#!/usr/bin/dotnet run

#:property OutputPath=./output
#:property TargetFramework=net11.0
#:property AppendTargetFrameworkToOutputPath=false
//#:property Optimize=true
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
    .AddStore<Rendereable<Texture2D>>()
    .AddStore<Obtainable>()
    .AddStore<Player>();

Texture2D appleTexture = Raylib.LoadTexture("assets/C_Logo.png");
Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

var appleComponent = new Rendereable<Texture2D>(
    appleTexture, 
    new(appleTexture.Width, appleTexture.Height),
    0.2f
);

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
            .AddComponent<Rendereable<Texture2D>>(world, new(basketTexture, new(basketTexture.Width, basketTexture.Height), 1));
            
var buffer = new List<World.Entity> (world.MaxValue);

while (!Raylib.WindowShouldClose())
{
    PlayerSystem.PlayerMove plyerMovement = PlayerSystem.PlayerMove.None;
    if (Raylib.IsKeyDown(KeyboardKey.W)) plyerMovement |= PlayerSystem.PlayerMove.Up;
    if (Raylib.IsKeyDown(KeyboardKey.A)) plyerMovement |= PlayerSystem.PlayerMove.Left;
    if (Raylib.IsKeyDown(KeyboardKey.S)) plyerMovement |= PlayerSystem.PlayerMove.Down;
    if (Raylib.IsKeyDown(KeyboardKey.D)) plyerMovement |= PlayerSystem.PlayerMove.Right;
    
    AppleSystem.RevictApples(buffer, geometryStore, obtainableStore, world);
    AppleSystem.RecicleApples(buffer, appleTexture, geometryStore, obtainableStore, world, tick, ref timer);

    GeometrySystem.Move(geometryStore);
    PlayerSystem.Move(player, geometryStore, plyerMovement);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    RenderSystem.RenderEntities (world, buffer, geometryStore, textures, DrawTextureScaled);

    Raylib.DrawFPS(0, 0);
    Raylib.EndDrawing();
}

static void DrawTextureScaled(Rendereable<Texture2D> rendereable, Position position)
{
    var source = new Rectangle(0, 0, rendereable.Texture2D.Width, rendereable.Texture2D.Height);
    var dest = new Rectangle(position.Point.X, position.Point.Y, rendereable.Texture2D.Width * rendereable.Scale, rendereable.Texture2D.Height * rendereable.Scale);
    Raylib.DrawTexturePro(rendereable.Texture2D, source, dest, new(0, 0), 0f, Color.White);
}

public record struct Obtainable() : IComponent;