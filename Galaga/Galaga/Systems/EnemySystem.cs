using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galaga.Systems;

public class EnemySystem : ObjectSystem
{
    private readonly PlayerSystem _playerSystem;
    private readonly BulletSystem _bulletSystem;

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
    }

    public override void Render(SpriteBatch spriteBatch)
    {
    }

    public override void ObjectHit(Guid id)
    {
    }
}