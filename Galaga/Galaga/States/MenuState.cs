using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Galaga.States;

public class MenuState : GameState
{
    private int _indexOfChoice;
    private List<string> _options;
    private GameStates _nextState;
    private Dictionary<int, Tuple<int, int>> _optionPositions;
    RenderTarget2D renderTarget;
    private AudioSystem _audioSystem;
    KeyboardState previousKeyboardState;
    private MouseState _previousMouseState;
    public bool UseAi;
    private TimeSpan _inactivityTimer = TimeSpan.Zero;

    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window)
    {
        _options = new List<string>
        {
            "Play Game",
            "High Scores",
            "Credits",
            "Controls",
            "Quit"
        };

        renderTarget = new RenderTarget2D(
            graphicsDevice,
            1440,
            1080,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            graphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        previousKeyboardState = Keyboard.GetState();
        _optionPositions = new Dictionary<int, Tuple<int, int>>();

        base.Initialize(graphicsDevice, graphics, window);
    }


    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("galaga", contentManager.Load<SpriteFont>("Fonts/File"));
        Fonts.Add("galagaBig", contentManager.Load<SpriteFont>("Fonts/File2"));
        Textures.Add("background", contentManager.Load<Texture2D>("Images/Background"));
        Textures.Add("galagaTitle", contentManager.Load<Texture2D>("Images/Galaga"));
        Fonts.Add("galagaSmall", contentManager.Load<SpriteFont>("Fonts/File3"));
        SoundEffects.Add("butterfly", contentManager.Load<SoundEffect>("Sound/Enemy2Death"));
        SoundEffects.Add("boss.1", contentManager.Load<SoundEffect>("Sound/Enemy3DeathPart1"));
        _audioSystem = new  AudioSystem(contentManager.Load<Song>("Sound/Startup"), SoundEffects);
    }

    public override GameStates Update(GameTime gameTime)
    {
        _inactivityTimer += gameTime.ElapsedGameTime;
        _nextState = GameStates.MainMenu;
        if (_inactivityTimer.Seconds > 10)
        {
            _inactivityTimer = TimeSpan.Zero;
            UseAi = true;
            return GameStates.GamePlay;
        }
        ProcessInput();
        return _nextState;
    }
    protected override void ProcessInput()
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        var initialIndex = _indexOfChoice;
        
        if (currentKeyboardState.IsKeyUp(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;

        if (currentKeyboardState.IsKeyUp(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down) && _indexOfChoice + 1 < _options.Count)
            _indexOfChoice += 1;

        if (!_optionPositions.Any())
            _optionPositions = MouseMenu.DefineOptionPositions(Fonts, _options.Count, renderTarget);
        var currentMouseState = Mouse.GetState();
        if (currentMouseState.Position != _previousMouseState.Position)
            _indexOfChoice = MouseMenu.UpdateIndexBasedOnMouse(currentMouseState, Window, _optionPositions, _indexOfChoice);
        
        if (currentKeyboardState.IsKeyUp(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter) 
            || currentMouseState.LeftButton == ButtonState.Released 
            && _previousMouseState.LeftButton == ButtonState.Pressed 
            && currentMouseState.X < Window.ClientBounds.Width
            && currentMouseState.X > 0)
        {
            _nextState = _indexOfChoice switch
            {
                0 => GameStates.GamePlay,
                1 => GameStates.HighScores,
                2 => GameStates.About,
                3 => GameStates.Controls,
                4 => GameStates.Exit,
                _ => throw new ArgumentOutOfRangeException()
            };
            // Handle enter key released
            _inactivityTimer = TimeSpan.Zero;
        }
        previousKeyboardState = currentKeyboardState;
        _previousMouseState = currentMouseState;

        base.ProcessInput();
        if (initialIndex != _indexOfChoice)
        {
            _inactivityTimer = TimeSpan.Zero;
            _audioSystem.PlaySoundEffect("boss.1");
        }
    }

    public override void Render()
    {
        Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(Textures["background"], new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
        SpriteBatch.Draw(Textures["galagaTitle"], new Rectangle(renderTarget.Width / 8, 20, 3 * renderTarget.Width /  4 , renderTarget.Height / 3), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        // Render Menu
        SpriteFont font = Fonts["galaga"];
        SpriteFont bigFont = Fonts["galagaBig"];
        int middle = _options.Count / 2;
        // Show options
        for (int i = 0; i < _options.Count; i++)
        {
            SpriteFont optionFont = _indexOfChoice == i ? bigFont : font;
            Vector2 stringSize = optionFont.MeasureString(_options[i]);
            int diff = i - middle;
            RenderUtilities.CreateBorderOnWord(SpriteBatch, optionFont, _options[i], new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + 100 + diff * Constants.MENU_BUFFER));
        }


        SpriteBatch.End();
        Graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(
                renderTarget,
                new Rectangle(Window.ClientBounds.Width / 8, 0, 4 * Window.ClientBounds.Height / 3, Window.ClientBounds.Height),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );
        SpriteBatch.End();
    }
}
