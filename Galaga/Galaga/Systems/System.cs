using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public abstract class System
{
    public abstract void LoadContent(ContentManager contentManager);
    public abstract void Update(GameTime gameTime);
    public abstract void Render(SpriteBatch spriteBatch);
}