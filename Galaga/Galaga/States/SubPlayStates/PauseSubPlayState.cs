using System;
using System.Collections.Generic;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
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
    private RenderTarget2D renderTarget;


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
        this.renderTarget = new RenderTarget2D(
            Graphics.GraphicsDevice,
            1440,
            1080,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
    }

    public override PlayStates Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.Down) && _indexOfChoice + 1 <= 1)
            _indexOfChoice += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            return _indexOfChoice == 0 ? PlayStates.Play : PlayStates.Finish;
        
        return PlayStates.Pause;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        // Show high score
        var font = fonts["default"];
        var bigFont = fonts["big"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(renderTarget.Width - stringSize.X, 0), Color.White);

        // Show options
        var optionFont = _indexOfChoice == 0 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[0]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[0], new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) - stringSize.Y));
        optionFont = _indexOfChoice == 1 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[1]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[1], new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + stringSize.Y));

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