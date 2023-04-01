using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    protected Dictionary<string, SpriteFont> Fonts;
    public abstract void LoadContent(ContentManager contentManager);
    public abstract PlayStates Update(GameTime gameTime);
    public abstract void Render(SpriteBatch spriteBatch);
}