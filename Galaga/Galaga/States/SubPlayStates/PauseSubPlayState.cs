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

public class PauseSubPlayState : SubPlayState
{
    private int _indexOfChoice;
    private readonly List<string> _options;
    private Dictionary<int, Tuple<int, int>> _optionPositions;
    private readonly HighScoreTracker _tracker;
    private readonly GraphicsDeviceManager Graphics;
    private readonly GameWindow Window;
    private readonly RenderTarget2D _renderTarget;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    private readonly IReadOnlyDictionary<string, Texture2D> _textures;

    private AudioSystem _audioSystem;

    public PauseSubPlayState(GraphicsDeviceManager graphics, GameWindow window, IReadOnlyDictionary<string, Texture2D> textures, AudioSystem audioSystem)
    {
        _options = new List<string>
        {
            "Resume",
            "Quit"
        };
        _tracker = HighScoreTracker.GetTracker();
        _optionPositions = new Dictionary<int, Tuple<int, int>>();

        Graphics = graphics;
        Window = window;
        _renderTarget = new RenderTarget2D(
            Graphics.GraphicsDevice,
            1440, // This is the size of all of the menues
            1080,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        _textures = textures;
        _previousKeyboardState = Keyboard.GetState();
        _audioSystem = audioSystem;
    }

    public override PlayStates Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        int initialIndex = _indexOfChoice;

        if (currentKeyboardState.IsKeyUp(Keys.Up) && _previousKeyboardState.IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;

        if (currentKeyboardState.IsKeyUp(Keys.Down) && _previousKeyboardState.IsKeyDown(Keys.Down) && _indexOfChoice + 1 < _options.Count)
            _indexOfChoice += 1;
        
        var currentMouseState = Mouse.GetState();
        if (currentMouseState.Position != _previousMouseState.Position)
            _indexOfChoice = MouseMenu.UpdateIndexBasedOnMouse(currentMouseState, Window, _optionPositions, _indexOfChoice);
        
        if (currentKeyboardState.IsKeyUp(Keys.Enter) && _previousKeyboardState.IsKeyDown(Keys.Enter)
            || currentMouseState.LeftButton == ButtonState.Released 
            && _previousMouseState.LeftButton == ButtonState.Pressed 
            && currentMouseState.X < Window.ClientBounds.Width
            && currentMouseState.X > 0)
            if (_indexOfChoice == 0)
            {
                _previousKeyboardState = currentKeyboardState;
                _previousMouseState = currentMouseState;
                _audioSystem.ResumeSounds();
                return PlayStates.Play;
            }
            else
            {
                _previousKeyboardState = currentKeyboardState;
                _previousMouseState = currentMouseState;
                return PlayStates.Finish;
            }

        _previousKeyboardState = currentKeyboardState;
        _previousMouseState = currentMouseState;

        if (initialIndex != _indexOfChoice)
        {
            _audioSystem.PlaySoundEffect("menuSelect", 0.5f);
        }
        return PlayStates.Pause;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        if (!_optionPositions.Any())
            _optionPositions = MouseMenu.DefineOptionPositions(fonts, _options.Count, _renderTarget);
        Graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
        Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_textures["background"], new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        // Show options
        SpriteFont font = fonts["galaga"];
        SpriteFont bigFont = fonts["galagaBig"];
        SpriteFont optionFont = _indexOfChoice == 0 ? bigFont : font;
        var stringSize = optionFont.MeasureString(_options[0]);
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
                new Rectangle(Window.ClientBounds.Width / 8, 0, 4 * Window.ClientBounds.Height / 3, Window.ClientBounds.Height),
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