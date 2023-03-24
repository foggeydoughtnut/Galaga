using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public abstract class System
{
    public abstract void Update(GameTime gameTime);
    public abstract void Render(SpriteBatch spriteBatch);
}