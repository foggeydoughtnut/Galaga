using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NotImplementedException = System.NotImplementedException;

namespace Galaga.Systems;

public class CollisionDetectionSystem : System
{
    private readonly PlayerSystem _playerSystem;
    private readonly EnemySystem _enemySystem;
    private readonly BulletSystem _bulletSystem;
    
    public CollisionDetectionSystem(PlayerSystem playerSystem, EnemySystem enemySystem, BulletSystem bulletSystem)
    {
        _playerSystem = playerSystem;
        _enemySystem = enemySystem;
        _bulletSystem = bulletSystem;
    }

    public override void LoadContent(ContentManager contentManager) { }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Render(SpriteBatch spriteBatch) { }
}