using cs_ecs;
using Raylib_cs;

namespace CatchApple;

internal static class Program
{
    const int screenWidth = 480;
    const int screenHeight = 800;

    static readonly World world = new(new(screenWidth, screenHeight), 100_000);

    [STAThread]
    public static void Main()
    {
        Build.BuildUtils.CopyDirectory("assets", Path.Combine("output", "assets"));
        Raylib.InitWindow(screenWidth, screenHeight, "Things");
        Texture2D appleTexture = Raylib.LoadTexture("assets/apple.png");
        Texture2D basketTexture = Raylib.LoadTexture("assets/basket.png");


        for (int i = 0; i < 8; i++)
        {
            var apple = world.AddEntityWith<Render, Position, Out>(new Render(appleTexture), new(new(0, 100 * i)), new ());
        }

        var player = world.AddEntityWith<Render, Position>(new Render(basketTexture), new(new(300, 300)));

        while (!Raylib.WindowShouldClose())
        {
            var renderables = new With<Render, Position>(world);
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

            RenderSystem.RenderEntities(renderables, DrawTextureScaled);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    static void DrawTextureScaled(Render rendereable, Position position, float scale)
    {
        var source = new Rectangle(0, 0, rendereable.Texture2D.Width, rendereable.Texture2D.Height);
        var dest = new Rectangle(position.Point.X, position.Point.Y, rendereable.Texture2D.Width * scale, rendereable.Texture2D.Height * scale);
        Raylib.DrawTexturePro(rendereable.Texture2D, source, dest, new(0, 0), 0f, Color.White);
    }
}