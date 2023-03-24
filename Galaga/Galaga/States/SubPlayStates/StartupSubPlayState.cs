using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.States.SubPlayStates;

public class StartupSubPlayState : ISubPlayState
{
    public StartupSubPlayState()
    {
        
    }
    
    public PlayStates Update(GameTime gameTime)
    {
        return PlayStates.Play;
    }

    public void Render(SpriteBatch spriteBatch) { }
    

}