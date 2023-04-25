using System;
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
    private AudioSystem _audioSystem;

    private bool _showIntroText;
    private bool _showIntroStageText;
    private float _introStageTextTimer;
    private bool _introStageTextTimerActive;
    private float _introStageTextDelay;
    private bool _showBonusRoundText;
    private TimeSpan _bonusRoundTextTimer;
    private int _splitBonusRoundCount;
    private TimeSpan _currentBonusRoundTextSplit;
    private bool _addedBonus;


    public PlaySubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures, AudioSystem audioSystem)
    {
        _showIntroText = false;
        _showIntroStageText = false;
        _introStageTextTimer = 0f;
        _introStageTextDelay = 5f;
        _bonusRoundTextTimer = new TimeSpan(0,0,0,6, 500);
        _splitBonusRoundCount = 0;
        _introStageTextTimerActive = false;
        _showBonusRoundText = false;

        _tracker = HighScoreTracker.GetTracker();
        _audioSystem = audioSystem;
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
        if (_audioSystem.IsPlayingMusic())
            _showIntroText = true;
        else
        {
            _showIntroText = false;
            _showIntroStageText = false;
        }
        if (_showIntroText)
        {
            _introStageTextTimerActive = true;
        }
        if (_introStageTextTimerActive)
        {
            _introStageTextTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_introStageTextTimer > _introStageTextDelay)
            {
                _showIntroStageText = true;
                _introStageTextTimer = 0f;
                _introStageTextTimerActive = false;
            }
        }

        if (EnemySystem.DisplayStageNumber)
        {
            _showBonusRoundText = EnemySystem.StageNumber % 3 == 1 && EnemySystem.StageNumber != 1;
            if (_showBonusRoundText)
            {
                _currentBonusRoundTextSplit += gameTime.ElapsedGameTime;
                if (_currentBonusRoundTextSplit > _bonusRoundTextTimer / 4)
                {
                    _currentBonusRoundTextSplit = TimeSpan.Zero;
                    _splitBonusRoundCount += 1;
                }
            }

            if (_splitBonusRoundCount >= 4)
            {
                _showBonusRoundText = false;
                if (!_addedBonus)
                {
                    _addedBonus = true;
                    _tracker.CurrentGameScore += EnemySystem.BonusRoundEnemiesDestroyed * 100;
                }
            }
        }
        else if (_splitBonusRoundCount >= 4)
        {
            _splitBonusRoundCount = 0;
            _addedBonus = false;
        }

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
        
        if (!_audioSystem.IsPlayingMusic())
        {
            foreach (Systems.System system in _systems)
            {
                system.Update(gameTime);
                if (system is PlayerSystem { PlayerKilled: true })
                    return PlayStates.Loser;
            }
        }
        if (currentKeyboardState.IsKeyUp(Keys.Escape) && _previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            _audioSystem.PauseSounds();
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
            else if (_showBonusRoundText)
            {
                var text = "Number of hits ";
                if (_splitBonusRoundCount > 0)
                    text += EnemySystem.BonusRoundEnemiesDestroyed;
                stringSize = font.MeasureString("Number of hits 23");
                spriteBatch.DrawString(font, text,
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y / 2 - 10 - stringSize.Y / 2), Color.Blue);

                if (_splitBonusRoundCount > 1)
                {
                    text = "Bonus ";
                    if (_splitBonusRoundCount > 2)
                        text += EnemySystem.BonusRoundEnemiesDestroyed * 100;
                    stringSize = font.MeasureString("Bonus 2300");
                    spriteBatch.DrawString(font, text,
                        new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y / 2 + 10 - stringSize.Y / 2), Color.Blue);
                }
            }
            else
            {
                stringSize = font.MeasureString($"Stage {EnemySystem.StageNumber}");
                spriteBatch.DrawString(font, $"Stage {EnemySystem.StageNumber}",
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y/2 - stringSize.Y/2), Color.Blue);
            }
        }
        if (_showIntroText)
        {
            if (!_showIntroStageText)
            {
                stringSize = font.MeasureString("Player 1");
                spriteBatch.DrawString(font, "Player 1",
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y / 2 - stringSize.Y / 2), Color.Blue);
            }
            else
            {
                stringSize = font.MeasureString("Stage 1");
                spriteBatch.DrawString(font, "Stage 1",
                    new Vector2(Constants.GAMEPLAY_X / 2 - stringSize.X / 2, Constants.GAMEPLAY_Y / 2 - stringSize.Y / 2), Color.Blue);
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