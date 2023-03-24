using System.Collections.Generic;
using CS5410.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class PauseSubPlayState : ISubPlayState
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

    public PlayStates Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
            _indexOfChoice -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.Down) && _indexOfChoice + 1 <= 1)
            _indexOfChoice += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            return _indexOfChoice == 0 ? PlayStates.Play : PlayStates.Loser;
        
        return PlayStates.Pause;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        
    }
}