using System;
using System.Collections.Generic;
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

    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        _options = new List<string>
        {
            "Play Game",
            "High Scores",
            "Credits",
            "Quit"
        };
        base.Initialize(graphicsDevice, graphics);
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
        if (IsKeyPressed(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;

        if (IsKeyPressed(Keys.Down) && _indexOfChoice + 1 < _options.Count)
            _indexOfChoice += 1;
        
        if (IsKeyPressed(Keys.Enter))
            _nextState =  _indexOfChoice switch
                { 
                    0=> GameStates.GamePlay,
                    1=> GameStates.HighScores,
                    2=> GameStates.About,
                    3=> GameStates.Exit,
                    _ => throw new ArgumentOutOfRangeException()
                }; 
        base.ProcessInput();
    }

    public override void Render()
    {
        SpriteBatch.Begin();
        var font = Fonts["default"];
        var bigFont = Fonts["veryBig"];
        var middle = _options.Count / 2;
        // Show options
        for (var i = 0; i < _options.Count; i++)
        {
            var optionFont = _indexOfChoice == i ? bigFont : font;
            var stringSize = optionFont.MeasureString(_options[i]);
            var diff = i - middle;
            RenderUtilities.CreateBorderOnWord(SpriteBatch, optionFont, _options[i], new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - stringSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 2) + diff * Constants.MENU_BUFFER));
        }
        SpriteBatch.End();
    }
}
