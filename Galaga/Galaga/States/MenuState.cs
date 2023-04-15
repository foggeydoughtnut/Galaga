using System;
using System.Collections.Generic;
using System.Diagnostics;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States;

public class MenuState : GameState
{
    private int _indexOfChoice;
    private List<string> _options;
    private GameStates _nextState;
    RenderTarget2D renderTarget;
    KeyboardState previousKeyboardState;

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

        this.renderTarget = new RenderTarget2D(
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

        base.Initialize(graphicsDevice, graphics, window);
    }


    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));
        Fonts.Add("veryBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
    }

    public override GameStates Update(GameTime gameTime)
    {
        _nextState = GameStates.MainMenu;
        ProcessInput();
        return _nextState;
    }
    protected override void ProcessInput()
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();



        if (currentKeyboardState.IsKeyUp(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;

        if (currentKeyboardState.IsKeyUp(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down) && _indexOfChoice + 1 < _options.Count)
            _indexOfChoice += 1;

        if (currentKeyboardState.IsKeyUp(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter))
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
        }
        previousKeyboardState = currentKeyboardState;

        base.ProcessInput();
    }

    public override void Render()
    {
        this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        // Render Menu
        SpriteFont font = Fonts["default"];
        SpriteFont bigFont = Fonts["big"];
        int middle = _options.Count / 2;
        // Show options
        for (int i = 0; i < _options.Count; i++)
        {
            SpriteFont optionFont = _indexOfChoice == i ? bigFont : font;
            Vector2 stringSize = optionFont.MeasureString(_options[i]);
            int diff = i - middle;
            RenderUtilities.CreateBorderOnWord(SpriteBatch, optionFont, _options[i], new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
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
