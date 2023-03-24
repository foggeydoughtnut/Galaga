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

    public PauseSubPlayState()
    {
        _options = new List<string>
        {
            "Resume",
            "Quit"
        };
        _tracker = HighScoreTracker.GetTracker();
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
        // Show high score
        var font = fonts["default"];
        var bigFont = fonts["big"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(Constants.BOUNDS_X - stringSize.X, 0), Color.White);

        // Show options
        var optionFont = _indexOfChoice == 0 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[0]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[0], new Vector2(Convert.ToInt32(Constants.BOUNDS_X / 2) - stringSize.X / 2, Convert.ToInt32(Constants.BOUNDS_Y / 2) - stringSize.Y));
        optionFont = _indexOfChoice == 1 ? bigFont : font;
        stringSize = optionFont.MeasureString(_options[1]);
        RenderUtilities.CreateBorderOnWord(spriteBatch, optionFont, _options[1], new Vector2(Convert.ToInt32(Constants.BOUNDS_X / 2) - stringSize.X / 2, Convert.ToInt32(Constants.BOUNDS_Y / 2) + stringSize.Y));
    }
}