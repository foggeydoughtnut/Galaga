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

public interface ISubPlayState
{
    public PlayStates Update(GameTime gameTime);
    public void Render(SpriteBatch spriteBatch);
}