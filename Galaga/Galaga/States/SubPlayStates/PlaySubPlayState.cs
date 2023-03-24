using System;
using System.Collections.Generic;
using System.Linq;
using CS5410.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class PlaySubPlayState : ISubPlayState
{
    private readonly Dictionary<int, int> _rowCounts = new();
    private readonly HighScoreTracker _tracker;
    
    public PlaySubPlayState()
    {
        _tracker = HighScoreTracker.GetTracker();
    }
    
    public PlayStates Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            return PlayStates.Loser;
        return PlayStates.Play;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        // Show high score
        // var font = fonts["default"];
        // var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        // spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
        //     new Vector2(graphics.PreferredBackBufferWidth - stringSize.X, 0), Color.White);
    }
}