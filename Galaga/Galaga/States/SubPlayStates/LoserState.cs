using System;
using System.Collections.Generic;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class LoserState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly GraphicsDeviceManager _graphics;
    private readonly GameWindow _window;
    private readonly RenderTarget2D _renderTarget;
    private readonly GameStatsSystem _gameStatsSystem;
    private KeyboardState _previousKeyboardState;
    public bool UsedAi;
    private readonly Dictionary<string, Texture2D> _textures;

    public LoserState(GraphicsDeviceManager graphics, GameWindow window, Dictionary<string, Texture2D> textures)
    {
        _tracker = HighScoreTracker.GetTracker();
        _gameStatsSystem = GameStatsSystem.GetSystem();
        _graphics = graphics;
        _window = window;
        _previousKeyboardState = Keyboard.GetState();
        _textures = textures;
        _renderTarget = new RenderTarget2D(
            graphics.GraphicsDevice,
            1440,
            1080,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
    }


    public override PlayStates Update(GameTime gameTime)
    {
        if (UsedAi)
        {
            _gameStatsSystem.FinishGame();
            _tracker.CurrentGameScore = 0;
            return PlayStates.Finish;
        }
        var currentKeyboardState = Keyboard.GetState();
        if (currentKeyboardState.GetPressedKeyCount() == 0 && _previousKeyboardState.GetPressedKeyCount() > 0)
        {
            _tracker.FinishGame();
            return PlayStates.Finish;
        } 
        _previousKeyboardState = currentKeyboardState;
        return PlayStates.Loser;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        _graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
        _graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_textures["background"], new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        var accuracy = (int)(_gameStatsSystem.ShotPercentage * 10000);
        var gameOverDisplay = "Game Over";
        var stringSize = fonts["galaga"].MeasureString(gameOverDisplay);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"], gameOverDisplay, new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(_renderTarget.Height / 4)));
        
        var totalShots = "Total Shots Fired: " + (_gameStatsSystem.HitBullets + _gameStatsSystem.MissedBullets);
        stringSize = fonts["galaga"].MeasureString(totalShots);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"], totalShots, new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(4 * _renderTarget.Height / 10)));
        
        var shotsHit = "Number of Hits: " + _gameStatsSystem.HitBullets;
        stringSize = fonts["galaga"].MeasureString(shotsHit);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"], shotsHit, new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(5 * _renderTarget.Height / 10)));
        
        var accuracyDisplay = "Accuracy: " + (float)accuracy / 100 + "%";
        stringSize = fonts["galaga"].MeasureString(accuracyDisplay);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"], accuracyDisplay , new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(6 * _renderTarget.Height / 10)));
        
        var keyToEscape = "Press any key to return to";
        stringSize = fonts["galaga"].MeasureString(keyToEscape);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"],keyToEscape , new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(7 * _renderTarget.Height / 10)));

        var main = "Main Menu";
        stringSize = fonts["galaga"].MeasureString(main);
        RenderUtilities.CreateBorderOnWord(spriteBatch, fonts["galaga"], main, new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(8 * _renderTarget.Height / 10)));
        
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