using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Object = Galaga.Objects.Object;

namespace Galaga.Systems;

public class AiSystem : System
{
    private readonly EnemySystem _enemySystem;
    private readonly PlayerShip _player;
    private readonly BulletSystem _bulletSystem;
    private readonly TimeSpan _bulletLimiter = new(0, 0, 0, 0, 350);
    private TimeSpan _timeSinceFire = TimeSpan.Zero;
    private Enemy _lastFiredEnemy;

    public AiSystem(EnemySystem enemySystem, PlayerSystem playerSystem, BulletSystem bulletSystem)
    {
        _enemySystem = enemySystem;
        _player = playerSystem.GetPlayer();
        _bulletSystem = bulletSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
        _timeSinceFire += gameTime.ElapsedGameTime;
        var enemies = _enemySystem.GetEnemies().ToList();
        if (!enemies.Any() || !enemies.Where(e=>e.Position.X is > 0 and < Constants.GAMEPLAY_X).Any())
        {
            MoveTowardsCenter(gameTime.ElapsedGameTime);
            return;
        }

        var enemyBullets = _bulletSystem.GetBullets().Where(b => b.VelocityY > 0).ToList();
        var attackingEnemies = enemies.Where(e => e.attack && e.Position.Y > 150).ToList();
        // If something is coming at us, avoid it
        if (enemyBullets.Any() || attackingEnemies.Any())
        {
            var objects = enemyBullets.Cast<Object>().ToList();
            objects.AddRange(attackingEnemies);
            var closestObject = FindClosestObject(objects);
            var distToObject = closestObject.Position.X - _player.Position.X; 
            if (Math.Abs(distToObject) < 50)
                MoveAwayFromIncoming(distToObject, _player.Position.Y - closestObject.Position.Y ,gameTime.ElapsedGameTime);
            else
                MoveTowardsCenter(gameTime.ElapsedGameTime);

            if(enemies.Where(e => Math.Abs(e.Position.X - _player.Position.X) < 5).Any())
                AttemptFire();
        }
        // Nothing is coming at us, so attack
        else
        {
            if(_lastFiredEnemy is not EnemyBossGalaga {health: 2} && enemies.Count > 1)
                enemies.Remove(_lastFiredEnemy);
            var closestEnemy = FindClosestXDirectionEnemy(enemies);
            var distToEnemy = _player.Position.X - closestEnemy.Position.X;
            switch (distToEnemy)
            {
                case > 2:
                    MoveLeft(gameTime.ElapsedGameTime);
                    break;
                case < -2:
                    MoveRight(gameTime.ElapsedGameTime);
                    break;
                default:
                    AttemptFire();
                    _lastFiredEnemy = closestEnemy;
                    break;
            }

            // Handle the challenging stage when they come out of the bottom
            var bottomOfEnemy = closestEnemy.Position.Y + closestEnemy.Dimensions.Y;
            if (bottomOfEnemy > _player.Position.Y && Math.Abs(distToEnemy) < 20)
            {
                if(distToEnemy > 0)
                    MoveRight(gameTime.ElapsedGameTime);
                else 
                    MoveLeft(gameTime.ElapsedGameTime);
            }
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        
    }

    private void MoveTowardsCenter(TimeSpan elapsedTime)
    {
        var distToCenter = _player.Position.X - Constants.GAMEPLAY_X / 2;
        switch (distToCenter)
        {
            // Move towards center
            case > 5:
                MoveLeft(elapsedTime);
                break;
            case < -5:
                MoveRight(elapsedTime);
                break;
        }
    }

    private void MoveAwayFromIncoming(int distToIncomingX, int distToIncomingY, TimeSpan elapsedTime)
    {
        switch (distToIncomingX)
        {
            case < 50 and > 0:
                MoveLeft(elapsedTime);
                break;
            case > -50 and < 0:
                MoveRight(elapsedTime);
                break;
        }
    }

    private void MoveRight(TimeSpan elapsedTime)
    {
        _player.MovePlayer(elapsedTime, 1);
    }

    private void MoveLeft(TimeSpan elapsedTime)
    {
        _player.MovePlayer(elapsedTime, -1);
    }

    
    private void AttemptFire()
    {
        if (_timeSinceFire < _bulletLimiter) return;
        _timeSinceFire = TimeSpan.Zero;
        var position = _player.Position;
        position.X += _player.Dimensions.X / 2;
        _bulletSystem.FirePlayerBullet(position);
    }

    private Object FindClosestObject(IEnumerable<Object> objects)
    {
        return objects.MinBy(a => Distance2DSquared(a.Position, _player.Position));
    }
    
    private Enemy FindClosestXDirectionEnemy(IEnumerable<Enemy> objects)
    {
        return objects.MinBy(a => Math.Abs(a.Position.X - _player.Position.X));
    }

    private static double Distance2DSquared(Point p1, Point p2)
    {
        var xDist = p1.X - p2.X;
        var yDist = p1.Y - p2.Y;
        return xDist * xDist + yDist * yDist;
    }
}