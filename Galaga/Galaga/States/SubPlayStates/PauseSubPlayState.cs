using System;
using System.Collections.Generic;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class PauseSubPlayState : SubPlayState
{
    private int _indexOfChoice;
    private readonly List<string> _options;
    private readonly HighScoreTracker _tracker;
    private readonly GraphicsDeviceManager Graphics;
    private readonly GameWindow Window;
    private readonly RenderTarget2D _renderTarget;
    private KeyboardState _previousKeyboardState;


    public PauseSubPlayState(GraphicsDeviceManager graphics, GameWindow window)
    {
        _options = new List<string>
        {
            "Resume",
            "Quit"
        };
        _tracker = HighScoreTracker.GetTracker();

        Graphics = graphics;
        Window = window;
        _renderTarget = new RenderTarget2D(
            Graphics.GraphicsDevice,
            Constants.GAMEPLAY_X,
            Constants.GAMEPLAY_Y,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        _previousKeyboardState = Keyboard.GetState();

    }

    public override PlayStates Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        if (currentKeyboardState.IsKeyUp(Keys.Up) && _previousKeyboardState.IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;

        if (currentKeyboardState.IsKeyUp(Keys.Down) && _previousKeyboardState.IsKeyDown(Keys.Down) && _indexOfChoice + 1 < _options.Count)
            _indexOfChoice += 1;
        if (currentKeyboardState.IsKeyUp(Keys.Enter) && _previousKeyboardState.IsKeyDown(Keys.Enter))
            if (_indexOfChoice == 0)
            {
                _previousKeyboardState = currentKeyboardState;
                return PlayStates.Play;
            }
            else
            {
                _previousKeyboardState = currentKeyboardState;
                return PlayStates.Finish;
            }

        _previousKeyboardState = currentKeyboardState;

        return PlayStates.Pause;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        Graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
        Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        // Show high score
        var font = fonts["default"];
        var bigFont = fonts["big"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(_renderTarget.Width - stringSize.X, 0), Color.White);

        // Show options
        var optionFont = _indexOfChoice == 0 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[0]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[0], new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(_renderTarget.Height / 2) - stringSize.Y));
        optionFont = _indexOfChoice == 1 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[1]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[1], new Vector2(Convert.ToInt32(_renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(_renderTarget.Height / 2) + stringSize.Y));

        spriteBatch.End();
        Graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(
                _renderTarget,
                new Rectangle(Window.ClientBounds.Width / 8, 0, 3 * Window.ClientBounds.Width / 4, Window.ClientBounds.Height),
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