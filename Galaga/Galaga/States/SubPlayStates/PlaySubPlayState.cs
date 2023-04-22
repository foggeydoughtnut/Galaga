using System.Collections.Generic;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private readonly IReadOnlyDictionary<string, Texture2D> _textures;
    private KeyboardState _previousKeyboardState;
    public bool UseAi;

    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures, IReadOnlyDictionary<string, SoundEffect> soundEffects)
    {
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        ParticleSystem particleSystem = new(textures["particle"]);
        GameStatsSystem gameStats = GameStatsSystem.GetSystem();
        BulletSystem bulletSystem = new(textures["playerBullet"], textures["enemyBullet"], gameStats, textures["debug"], soundEffects);
        PlayerSystem playerSystem = new(textures["ship"], gameStats, bulletSystem, textures["debug"], particleSystem, soundEffects);

        EnemySystem enemySystem = new(playerSystem, bulletSystem, particleSystem, window, textures["bee"], textures["debug"], textures["butterfly"], new() { textures["bossGalagaFull"], textures["bossGalagaHalf"] }, textures["dragonfly"], textures["satellite"], soundEffects);
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
        if (UseAi)
            SetupAi();
        KeyboardState currentKeyboardState = Keyboard.GetState();
        if (UseAi && _previousKeyboardState.GetPressedKeys().Any() && !currentKeyboardState.GetPressedKeys().Any())
            return PlayStates.Loser;
        foreach (Systems.System system in _systems)
        {
            system.Update(gameTime);
            if (system is PlayerSystem { PlayerKilled: true })
                return PlayStates.Loser;
        }
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

    private void SetupAi()
    {
        if (_systems.OfType<AiSystem>().Any()) return;
        var enemySystem = _systems.OfType<EnemySystem>().First();
        var playerSystem = _systems.OfType<PlayerSystem>().First();
        var bulletSystem = _systems.OfType<BulletSystem>().First();
        _systems.Add(new AiSystem(enemySystem, playerSystem, bulletSystem));
    }
}