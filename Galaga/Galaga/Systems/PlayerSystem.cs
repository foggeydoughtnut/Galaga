using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public class PlayerSystem : ObjectSystem
{
    private readonly BulletSystem _bulletSystem;
    private readonly GameStatsSystem _gameStatsSystem;
    public PlayerSystem(GameStatsSystem gameStatsSystem, BulletSystem bulletSystem)
    {
        _bulletSystem = bulletSystem;
        _gameStatsSystem = gameStatsSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }

    public override void ObjectHit(int id)
    {
        throw new NotImplementedException();
    }
}