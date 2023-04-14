using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Galaga.States.SubPlayStates;

public class PlaySubPlayState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly List<Systems.System> _systems;
    private readonly GraphicsDeviceManager Graphics;
    private readonly GameWindow Window;
    private RenderTarget2D renderTarget;
    private readonly IReadOnlyDictionary<string, Texture2D> _textures;
    KeyboardState previousKeyboardState;


    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures)
    {
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        ParticleSystem particleSystem = new(textures["particle"]);
        var gameStats = new GameStatsSystem();
        var bulletSystem = new BulletSystem(textures["playerBullet"], textures["enemyBullet"], gameStats, textures["debug"]);
        var playerSystem = new PlayerSystem(textures["ship"], gameStats, bulletSystem, textures["debug"], particleSystem);
        //var playerSystem = new PlayerSystem(textures["bossGalagaHalf"], gameStats, bulletSystem, textures["debug"], particleSystem); // FOR TESTING ANIMATION DELETE

        var enemySystem = new EnemySystem(playerSystem, bulletSystem);
        var collisionDetectionSystem = new CollisionDetectionSystem(playerSystem, enemySystem, bulletSystem);
        _systems.Add(playerSystem);
        _systems.Add(enemySystem);
        _systems.Add(bulletSystem);
        _systems.Add(collisionDetectionSystem);
        _systems.Add(particleSystem);

        Graphics = graphics;
        Window = window;
        this.renderTarget = new RenderTarget2D(
            Graphics.GraphicsDevice,
            Constants.GAMEPLAY_Y,
            Constants.GAMEPLAY_X,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        _textures = textures;
        previousKeyboardState = Keyboard.GetState();

    }

    public override PlayStates Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        foreach (var system in _systems)
            system.Update(gameTime);
        //Debug.WriteLine($"Key up: {currentKeyboardState.IsKeyUp(Keys.Escape)} Key down: {previousKeyboardState.IsKeyDown(Keys.Escape)}");

        if (currentKeyboardState.IsKeyUp(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            previousKeyboardState = currentKeyboardState;
            return PlayStates.Pause;
        }

        previousKeyboardState = currentKeyboardState;
        return PlayStates.Play;

    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        this.Graphics.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_textures["background"], new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        foreach (var system in _systems)
            system.Render(spriteBatch);

        // Show high score
        var font = fonts["default"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(renderTarget.Width - stringSize.X / 2, 0), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);


        spriteBatch.End();
        Graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(
                renderTarget,
                new Rectangle(Window.ClientBounds.Width / 8, 0, (Window.ClientBounds.Height / 3 * 4), Window.ClientBounds.Height),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );
        spriteBatch.End();
    }
}