using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class PlaySubPlayState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly List<Systems.System> _systems;
    private readonly GraphicsDeviceManager _graphics;
    private readonly GameWindow _window;
    private readonly RenderTarget2D _renderTarget;
    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window)
    {
        Fonts = new Dictionary<string, SpriteFont>();
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        var gameStats = new GameStatsSystem();
        var bulletSystem = new BulletSystem(gameStats);
        var playerSystem = new PlayerSystem(gameStats, bulletSystem);
        var enemySystem = new EnemySystem(playerSystem, bulletSystem, window);
        var collisionDetectionSystem = new CollisionDetectionSystem(playerSystem, enemySystem, bulletSystem);
        _systems.Add(playerSystem);
        _systems.Add(enemySystem);
        _systems.Add(bulletSystem);
        _systems.Add(collisionDetectionSystem);

        _graphics = graphics;
        _window = window;
        _renderTarget = new RenderTarget2D(
            _graphics.GraphicsDevice,
            Constants.GAMEPLAY_X,
            Constants.GAMEPLAY_Y,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            _graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
    }

    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));
        Fonts.Add("vBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
        
        foreach(var system in _systems)
            system.LoadContent(contentManager);
    }

    public override PlayStates Update(GameTime gameTime)
    {
        foreach(var system in _systems)
            system.Update(gameTime);
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            return PlayStates.Pause;
        return PlayStates.Play;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        _graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
        _graphics.GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        foreach (var system in _systems)
            system.Render(spriteBatch);

        // Show high score
        var font = Fonts["default"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(_renderTarget.Width - stringSize.X, 0), Color.White);


        spriteBatch.End();
        _graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(
                _renderTarget,
                new Rectangle(_window.ClientBounds.Width / 8, 0, 3 * _window.ClientBounds.Width / 4, _window.ClientBounds.Height),
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