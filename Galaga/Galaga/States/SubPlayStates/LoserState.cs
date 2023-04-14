using System.Collections.Generic;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class LoserState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly GraphicsDeviceManager _graphics;
    private readonly GameWindow _window;

    public LoserState(GraphicsDeviceManager graphics, GameWindow window)
    {
        _tracker = HighScoreTracker.GetTracker();
        _graphics = graphics;
        _window = window;
    }


    public override PlayStates Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyUp(Keys.Enter)) return PlayStates.Loser;
        _tracker.FinishGame();
        return PlayStates.Finish;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
    }
}