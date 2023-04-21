using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.States.SubPlayStates;


public enum PlayStates
{
    Play,
    Loser,
    Pause,
    Finish
}

public abstract class SubPlayState
{
    public abstract PlayStates Update(GameTime gameTime);
    public abstract void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts);
}