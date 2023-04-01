using Galaga.Objects;
using Microsoft.Xna.Framework;
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
    public List<Tuple<Guid, string>> _collisions { get; private set; } // List of tuples that keeps track of which objects had collisions
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
            gameObjects.Add(new (bullets[i].Id, bullets[i].Collider, "bullet"));
        }

        // TODO: Add enemies to the collisions too


        for (int i = 0; i < gameObjects.Count; i++)
        {
            for (int j = 0; j < gameObjects.Count; j++)
            {
                if (gameObjects[i].Item1 == gameObjects[j].Item1) continue;
                // Check for each object, do they collide with the next object
                bool intersects = gameObjects[i].Item2.Intersects(gameObjects[j].Item2);
                if (intersects)
                {
                    // If they do, mark that both of add to the collisions dictionary and mark that both collided
                    _collisions.Add(new(gameObjects[i].Item1, gameObjects[i].Item3));
                    _collisions.Add(new(gameObjects[j].Item1, gameObjects[j].Item3));

                }
            }
        }

        // Go through each collision and notify the system that it had a collision
        for (int i = 0; i < _collisions.Count; i++)
        {
            Guid id = _collisions[i].Item1;
            string type = _collisions[i].Item2;

            if (type == "player")
            {
                _playerSystem.PlayerHit();
            }
            else if (type == "bullet")
            {
                _bulletSystem.ObjectHit(id);
            }
            else if (type == "enemy")
            {
                //TODO notify enemy that it was hit
            }
        }
    }

    public override void Render(SpriteBatch spriteBatch) { }
}