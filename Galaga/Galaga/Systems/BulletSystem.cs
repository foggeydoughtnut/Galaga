using System.Collections.Generic;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public class BulletSystem : ObjectSystem
{
    private readonly GameStatsSystem _statsSystem;
    private readonly List<Bullet> _bullets = new();
    private readonly Texture2D _playerBulletTexture;
    private readonly Texture2D _enemyBulletTexture;
    
    public BulletSystem(Texture2D playerBulletTexture, Texture2D enemyBulletTexture,GameStatsSystem statsSystem)
    {
        _statsSystem = statsSystem;
        _playerBulletTexture = playerBulletTexture;
        _enemyBulletTexture = enemyBulletTexture;
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var bullet in _bullets)
            bullet.Update(gameTime.ElapsedGameTime);
        _bullets.RemoveAll(b => b.Position.Y is <= 0 or > Constants.BOUNDS_Y);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        foreach(var bullet in _bullets)
            bullet.Render(spriteBatch);
    }

    public override void ObjectHit(int id)
    {
    }

    public void FirePlayerBullet(Point position)
    {
        _bullets.Add(new Bullet(position, new Point(25,25), _playerBulletTexture, -1500));
    }
    
    public void FireEnemyBullet(Point position)
    {
        _bullets.Add(new Bullet(position, new Point(25,25), _playerBulletTexture, 1500));
    }
}