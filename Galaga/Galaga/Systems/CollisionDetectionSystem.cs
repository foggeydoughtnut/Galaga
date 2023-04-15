using Galaga.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Galaga.Systems;

public class CollisionDetectionSystem : System
{
    private readonly PlayerSystem _playerSystem;
    private readonly EnemySystem _enemySystem;
    private readonly BulletSystem _bulletSystem;
    //public List<Tuple<Guid, string>> _collisions { get; private set; } // List of tuples that keeps track of which objects had collisions
    private Dictionary<Guid, string> _collisions; // List of tuples that keeps track of which objects had collisions

    private List<Tuple<Guid, Rectangle, string>> gameObjects;
    
    public CollisionDetectionSystem(PlayerSystem playerSystem, EnemySystem enemySystem, BulletSystem bulletSystem)
    {
        _playerSystem = playerSystem;
        _enemySystem = enemySystem;
        _bulletSystem = bulletSystem;
        _collisions = new();
        gameObjects = new();
    }

    public override void Update(GameTime gameTime)
    {
        // Get all of the objects' colliders in each of the systems
        gameObjects.Clear();
        _collisions.Clear();

        PlayerShip playerShip = _playerSystem.GetPlayer();
        gameObjects.Add(new (playerShip.Id, playerShip.Collider, "player"));

        List<Bullet> bullets = _bulletSystem.GetBullets();
        for (int i = 0; i < bullets.Count; i++)
        {
            gameObjects.Add(new(bullets[i].Id, bullets[i].Collider, "bullet"));
        }

        foreach(Enemy enemy in _enemySystem.GetEnemies())
            gameObjects.Add(new Tuple<Guid, Rectangle, string>(enemy.Id, enemy.Collider, "enemy"));
        
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (_collisions.ContainsKey(gameObjects[i].Item1)) continue;
            for (int j = 0; j < gameObjects.Count; j++)
            {
                if (gameObjects[i].Item1 == gameObjects[j].Item1) continue;
                if (_collisions.ContainsKey(gameObjects[j].Item1)) continue;
                if (gameObjects[i].Item3 == "bullet" && gameObjects[j].Item3 == "bullet") continue; // Two bullets can't collide
                if (gameObjects[i].Item3 == "enemy" && gameObjects[j].Item3 == "enemy") continue; // Two enemies can't collide
                bool intersects = gameObjects[i].Item2.Intersects(gameObjects[j].Item2);
                if (intersects)
                {
                    // If they do, mark that both of add to the collisions dictionary and mark that both collided
                    _collisions.Add(gameObjects[i].Item1, gameObjects[i].Item3);
                    _collisions.Add(gameObjects[j].Item1, gameObjects[j].Item3);
                }
            }
        }

        // Go through each collision and notify the system that it had a collision
        foreach (KeyValuePair<Guid, string> collision in _collisions)
        {
            if (collision.Value == "player")
            {
                _playerSystem.PlayerHit();
            }
            else if (collision.Value == "bullet")
            {
                _bulletSystem.ObjectHit(collision.Key);
            }
            else if (collision.Value == "enemy")
            {
                _enemySystem.ObjectHit(collision.Key);
            }
        }
    }

    public override void Render(SpriteBatch spriteBatch) { }
}