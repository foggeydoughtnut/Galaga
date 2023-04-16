using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Galaga.States.SubPlayStates;

public class PlaySubPlayState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly List<Systems.System> _systems;
    private readonly GraphicsDeviceManager _graphics;
    private readonly GameWindow _window;
    private readonly RenderTarget2D _renderTarget;
    private readonly IReadOnlyDictionary<string, Texture2D> _textures;
    private KeyboardState _previousKeyboardState;

    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures)
    {
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        ParticleSystem particleSystem = new(textures["particle"]);
        GameStatsSystem gameStats = new();
        BulletSystem bulletSystem = new(textures["playerBullet"], textures["enemyBullet"], gameStats, textures["debug"]);
        PlayerSystem playerSystem = new(textures["ship"], gameStats, bulletSystem, textures["debug"], particleSystem);

        EnemySystem enemySystem = new(playerSystem, bulletSystem, particleSystem, window, textures["bee"], textures["debug"], textures["butterfly"]);
        CollisionDetectionSystem collisionDetectionSystem = new(playerSystem, enemySystem, bulletSystem);
        _systems.Add(playerSystem);
        _systems.Add(enemySystem);
        _systems.Add(bulletSystem);
        _systems.Add(collisionDetectionSystem);
        _systems.Add(particleSystem);

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
        _textures = textures;
        _previousKeyboardState = Keyboard.GetState();

    }

    public override PlayStates Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        foreach (Systems.System system in _systems)
            system.Update(gameTime);
        if (currentKeyboardState.IsKeyUp(Keys.Escape) && _previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _previousKeyboardState = currentKeyboardState;
            return PlayStates.Pause;
        }

        _previousKeyboardState = currentKeyboardState;
        return PlayStates.Play;

    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        _graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
        _graphics.GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        _graphics.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_textures["background"], new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        foreach (Systems.System system in _systems)
            system.Render(spriteBatch);

        // Show high score
/*        SpriteFont font = fonts["default"];
        Vector2 stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(_renderTarget.Width - stringSize.X, 0), Color.White);

*/
        spriteBatch.End();
        _graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(
                _renderTarget,
                new Rectangle(_window.ClientBounds.Width / 8, 0, 4 * _window.ClientBounds.Height / 3, _window.ClientBounds.Height),
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