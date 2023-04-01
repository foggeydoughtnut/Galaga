using System.Collections.Generic;
using System.Diagnostics;
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
    private readonly Texture2D _debugTexture;

    private bool shot = false; // THIS IS BAD SO DELETE THIS!!!!


    public BulletSystem(Texture2D playerBulletTexture, Texture2D enemyBulletTexture,GameStatsSystem statsSystem, Texture2D debugTexture)
    {
        _statsSystem = statsSystem;
        _playerBulletTexture = playerBulletTexture;
        _enemyBulletTexture = enemyBulletTexture;
        _debugTexture = debugTexture;
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
        //if (!shot) // DELETE THIS
        //{ // DELETE THIS
      _bullets.Add(new Bullet(new Point(position.X-1, position.Y-6), new Point(_playerBulletTexture.Width, _playerBulletTexture.Height), _playerBulletTexture, -250, _debugTexture));
            //_bullets.Add(new Bullet(new Point(position.X, 0), new Point(_enemyBulletTexture.Width, _enemyBulletTexture.Height), _enemyBulletTexture, 250, _debugTexture)); // DELETE THIS
            //shot = true; // DELETE THIS
        //} // DELETE THIS

    }
    
    public void FireEnemyBullet(Point position)
    {
        _bullets.Add(new Bullet(position, new Point(25,25), _playerBulletTexture, 1500, _debugTexture));
    }
}