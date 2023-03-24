using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public class BulletSystem : ObjectSystem
{
    private readonly GameStatsSystem _statsSystem;
    public BulletSystem(GameStatsSystem statsSystem)
    {
        _statsSystem = statsSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
    }

    public override void Render(SpriteBatch spriteBatch)
    {
    }

    public override void ObjectHit(int id)
    {
    }
}