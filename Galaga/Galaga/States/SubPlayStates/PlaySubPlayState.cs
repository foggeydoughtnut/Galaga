using System.Collections.Generic;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
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
    private MouseState _previousMouseState;
    public bool UseAi;

    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures, AudioSystem audioSystem)
    {
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        ParticleSystem particleSystem = new(textures["particle"]);
        GameStatsSystem gameStats = GameStatsSystem.GetSystem();
        BulletSystem bulletSystem = new(textures["playerBullet"], textures["enemyBullet"], gameStats, textures["debug"], audioSystem);
        PlayerSystem playerSystem = new(textures["ship"], gameStats, bulletSystem, textures["debug"], particleSystem, audioSystem);

        EnemySystem enemySystem = new(playerSystem, bulletSystem, particleSystem, window, textures["bee"], textures["debug"], textures["butterfly"], new() { textures["bossGalagaFull"], textures["bossGalagaHalf"] }, textures["dragonfly"], textures["satellite"], audioSystem);
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
            Constants.RENDER_TARGET_X,
            Constants.RENDER_TARGET_Y,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            _graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        _textures = textures;
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();

    }

    public override PlayStates Update(GameTime gameTime)
    {
        if (UseAi)
            SetupAi();
        KeyboardState currentKeyboardState = Keyboard.GetState();
        MouseState currentMouseState = Mouse.GetState();
        if (UseAi)
        {
            var buttonPressed = _previousKeyboardState.GetPressedKeys().Any() && !currentKeyboardState.GetPressedKeys().Any();
            var mouseMoved = _previousMouseState != currentMouseState && currentMouseState.Position is { X: < Constants.BOUNDS_X and > 0, Y: < Constants.BOUNDS_Y and > 0} ;
            if(buttonPressed || mouseMoved)
                return PlayStates.Loser;
        }
            
        foreach (Systems.System system in _systems)
        {
            system.Update(gameTime);
            if (system is PlayerSystem { PlayerKilled: true })
                return PlayStates.Loser;
        }
        if (currentKeyboardState.IsKeyUp(Keys.Escape) && _previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            return PlayStates.Pause;
        }

        _previousKeyboardState = currentKeyboardState;
        _previousMouseState = currentMouseState;
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

        SpriteFont font = fonts["galagaSmall"];
        Vector2 stringSize = font.MeasureString(_tracker.CurrentGameScore.ToString());
        
        spriteBatch.DrawString(font, "1UP",
            new Vector2(font.MeasureString("1").X, 0), Color.Red);
        spriteBatch.DrawString(font, _tracker.CurrentGameScore.ToString(),
            new Vector2(0, stringSize.Y), Color.White);

        stringSize = font.MeasureString("High Score");
        spriteBatch.DrawString(font, "High Score",
            new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, 0), Color.Red);
        var highScore = "20000";
        if (_tracker.HighScores.Any())
            highScore = _tracker.HighScores.Max().ToString();
        stringSize = font.MeasureString(highScore);
        spriteBatch.DrawString(font, highScore,
            new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, stringSize.Y), Color.White);
        
        if (EnemySystem.DisplayStageNumber)
        {
            if (EnemySystem.StageNumber % 3 == 0)
            {
                stringSize = font.MeasureString("Challenging Stage");
                spriteBatch.DrawString(font, "Challenging Stage",
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y / 2 - stringSize.Y / 2), Color.Blue);
            }
            else
            {
                stringSize = font.MeasureString($"Stage {EnemySystem.StageNumber}");
                spriteBatch.DrawString(font, $"Stage {EnemySystem.StageNumber}",
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y/2 - stringSize.Y/2), Color.Blue);
            }
        }
        
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
        // Show high score
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