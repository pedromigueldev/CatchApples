#!/usr/bin/dotnet run

#:package Raylib-cs@8.0.0
#:property OutputPath=./output
#:property TargetFramework=net11.0
#:property AppendTargetFrameworkToOutputPath=false
#:property IlcOptimizationPreference=Speed
#:property WarningsAsErrors=nullable
#:property ImplicitUsings=enable
#:property OptimizationPreference=Speed
//#:property Optimize=true // removes debug info

#:include vendor/**/*.cs
#:include systems/*.cs
#:include build.cs

using CatchApple;
using Cecs;
using Raylib_cs;

internal static class Program
{
    const int screenWidth = 480;
    const int screenHeight = 800;

    public record struct Render(Texture2D Texture2D) : IComponent;
    static readonly World world = new World(new(screenWidth, screenHeight), 100_000)
        .AddStore<Obtainable>()
        .AddStore<Player>()
        .AddStore<Hitbox>()
        .AddStore<Out>()
        .AddStore<Velocity>()
        .AddComponentGroup<Render, Position>()
    ;
    
    [STAThread]
    public static void Main()
    {
        Build.BuildUtils.CopyDirectory("assets", Path.Combine("output", "assets"));
        Raylib.InitWindow(screenWidth, screenHeight, "Things");
        Texture2D appleTexture = Raylib.LoadTexture("assets/apple.png");
        Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");

        var Renderable = world.GetComponentGroup<Render, Position>();

        var apple = world.CreateEntity()
                .AddEntityWith(Renderable, new Render(appleTexture), new (new (10,10)))
                .AddEntityWith(world.GetStore<Obtainable>())
                .AddEntityWith(world.GetStore<Hitbox>())
                .AddEntityWith(world.GetStore<Out>());

        var player = world.CreateEntity()
                .AddEntityWith(Renderable, new Render(basketTexture), new (new (100,100)))
                .AddEntityWith(world.GetStore<Hitbox>());

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

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