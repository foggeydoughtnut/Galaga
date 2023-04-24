using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public class BulletSystem : ObjectSystem
{
    private readonly GameStatsSystem _statsSystem;
    private readonly List<Bullet> _bullets = new();
    private readonly Texture2D _playerBulletTexture;
    private readonly Texture2D _enemyBulletTexture;
    private readonly Texture2D _debugTexture;
    private readonly AudioSystem _audioSystem;

    private int _numberOfPlayerBulletsOut; // In the game you can only have two player bullets out on the field at once. 


    public List<Bullet> GetBullets() { return _bullets; }

    public BulletSystem(Texture2D playerBulletTexture, Texture2D enemyBulletTexture, GameStatsSystem statsSystem, Texture2D debugTexture, AudioSystem audioSystem)
    {
        _statsSystem = statsSystem;
        _playerBulletTexture = playerBulletTexture;
        _enemyBulletTexture = enemyBulletTexture;
        _debugTexture = debugTexture;
        _numberOfPlayerBulletsOut = 0;
        _audioSystem = audioSystem;
    }

    public override void Update(GameTime gameTime)
    {
        foreach (Bullet bullet in _bullets)
            bullet.Update(gameTime.ElapsedGameTime);
        _bullets.RemoveAll(delegate (Bullet b)
        {
            if (b.Position.Y is <= 0 or > Constants.GAMEPLAY_Y)
            {
                if (b.VelocityY < 0) // Player bullets are always negative velocity and enemies are positive
                {
                    _numberOfPlayerBulletsOut--;
                    _statsSystem.MissedBullet();
                }
                return true;
            }
            return false;
        });
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        foreach(Bullet bullet in _bullets)
            bullet.Render(spriteBatch);
    }

    public void ObjectHit(Guid id)
    {
        _bullets.RemoveAll(delegate(Bullet e) 
        {
            if (e.Id == id)
            {
                if (e.VelocityY < 0) // Player bullets are always negative velocity and enemies are positive
                {
                    _numberOfPlayerBulletsOut--;
                    _statsSystem.HitBullet();
                }
                return true;
            }
            return false;
        });

    }

    public void FirePlayerBullet(Point position)
    {
        if (_numberOfPlayerBulletsOut < 2) // In the game you could only have two bullets out at a time for added difficulty but this one he said he wanted it not to be limited
        {
            _audioSystem.PlaySoundEffect("shot");
            _bullets.Add(new Bullet(
                position: new Point(position.X-1, position.Y-6),
                dimensions: new Point(_playerBulletTexture.Width, _playerBulletTexture.Height),
                _playerBulletTexture,
                velocity: -250,
                _debugTexture,
                numberOfSubImages: 1,
                "player"
            ));
            _numberOfPlayerBulletsOut++;
        }
        //_bullets.Add(new Bullet(new Point(position.X-1, 0), new Point(_enemyBulletTexture.Width, _enemyBulletTexture.Height), _enemyBulletTexture, 250, _debugTexture, 1)); // DELETE THIS

    }
    
    public void FireEnemyBullet(Point position, Vector2 velocity)
    {
        _bullets.Add(new Bullet(
            position,
            dimensions: new Point(_enemyBulletTexture.Width, _enemyBulletTexture.Height),
            _playerBulletTexture,
            velocity: velocity * 200,
            _debugTexture,
            numberOfSubImages: 1,
            "enemy"
        ));
    }
}