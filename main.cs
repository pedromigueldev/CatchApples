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

public readonly record struct Obtainable() : IComponent;
public readonly record struct Out() : IComponent;
internal static class Program
{
    const int screenWidth = 480;
    const int screenHeight = 800;
    static World world = new World(new(screenWidth, screenHeight), 100_000)
        .AddStore<Obtainable>()
        .AddStore<Player>()
        .AddStore<Hitbox>()
        .AddStore<Out>()
        .AddStore<Velocity>()
        .AddArchetype<(Position, float, Texture2D)>() // renderable (scale, texture)
    ;
    
    [STAThread]
    public static void Main()
    {
        Build.BuildUtils.CopyDirectory("assets", Path.Combine("output", "assets"));
        Raylib.InitWindow(screenWidth, screenHeight, "Things");
        Texture2D appleTexture = Raylib.LoadTexture("assets/apple.png");
        Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

        var Renderable = world.GetArchetype<(Position, float, Texture2D)>();

        var apple = world.CreateEntity()
                .AddEntityWith(Renderable, (new (new (10,10)), 1f, appleTexture))
                .AddEntityWith(world.GetStore<Obtainable>())
                .AddEntityWith(world.GetStore<Hitbox>())
                .AddEntityWith(world.GetStore<Out>());

        var player = world.CreateEntity()
                .AddEntityWith(Renderable, (new (new (100,100)), 1f, basketTexture))
                .AddEntityWith(world.GetStore<Hitbox>());

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

            RenderSystem.RenderEntities(Renderable, DrawTextureScaled);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    static void DrawTextureScaled(Texture2D rendereable, Position position, float scale)
    {
        var source = new Rectangle(0, 0, rendereable.Width, rendereable.Height);
        var dest = new Rectangle(position.Point.X, position.Point.Y, rendereable.Width * scale, rendereable.Height * scale);
        Raylib.DrawTexturePro(rendereable, source, dest, new(0, 0), 0f, Color.White);
    }
}