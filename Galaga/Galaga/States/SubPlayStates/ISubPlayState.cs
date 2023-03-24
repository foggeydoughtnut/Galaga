using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.States.SubPlayStates;


public enum PlayStates
{
    Startup,
    Play,
    Loser,
    Pause
}

public interface ISubPlayState
{
    public PlayStates Update(GameTime gameTime);
    public void Render(SpriteBatch spriteBatch);
}