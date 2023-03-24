using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class LoserState : ISubPlayState
{
    private readonly HighScoreTracker _tracker;

    public LoserState()
    {
        _tracker = HighScoreTracker.GetTracker();
    }    

    public PlayStates Update(GameTime gameTime)
    {
        if (!Keyboard.GetState().IsKeyDown(Keys.Enter)) return PlayStates.Loser;
        _tracker.FinishGame();
        return PlayStates.Finish;
    }

    public void Render(SpriteBatch spriteBatch)
    {
    }
}