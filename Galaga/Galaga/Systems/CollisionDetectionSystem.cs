using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    
    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Render(SpriteBatch spriteBatch) { }
}