using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;

public class EnemySystem : ObjectSystem
{
    private readonly PlayerSystem _playerSystem;
    private readonly BulletSystem _bulletSystem;
    private readonly ParticleSystem _particleSystem;
    private readonly List<Enemy> _enemies;
    private readonly GameWindow _window;
    private const int EntranceCircleRadius = Constants.GAMEPLAY_X / 8;
    private Vector2 _butterflyNextPos;
    private Vector2 _beeNextPos;
    private Vector2 _bossGalagaNextPos;
    private List<Vector2> _points;
    private readonly TimeSpan _entranceDelay = new(0,0,0,0,150);
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private readonly Texture2D _beeTexture;
    private readonly Texture2D _butterflyTexture;
    private readonly Texture2D _bossGalagaTexture;

    private readonly Texture2D _debugTexture;
    private readonly int _maxEnemies = 10;
    private int _createdEnemies;

    public IEnumerable<Enemy> GetEnemies() => _enemies.ToList();

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem, ParticleSystem particleSystem, GameWindow window, Texture2D beeTexture, Texture2D debugTexture, Texture2D butterflyTexture, Texture2D bossGalagaTexture)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _particleSystem = particleSystem;
        _beeTexture = beeTexture;
        _debugTexture = debugTexture;
        _butterflyTexture = butterflyTexture;
        _bossGalagaTexture = bossGalagaTexture;

        _enemies = new List<Enemy>();
        _window = window;
        _bossGalagaNextPos = new Vector2(50.0f, 18.0f);
        _butterflyNextPos = new Vector2(50.0f, 50.0f);
        _beeNextPos = new Vector2(50.0f, 82.0f);

        _points = new List<Vector2> { new(200, 0) };
        Random rand = new();
        int randX = rand.Next() % (Constants.GAMEPLAY_X / 2) + Constants.GAMEPLAY_X / 4;
        int randY = rand.Next() % (Constants.GAMEPLAY_Y / 4) + Constants.GAMEPLAY_Y / 2;
        _points.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(randX, randY, EntranceCircleRadius));
    }

    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemies)
        {
            _createdEnemies++;
            EnemyBossGalaga newBossGalaga = new(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                _bossGalagaTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
            {
                EntrancePath = _points.ToList(),
                Destination = _bossGalagaNextPos
            };
            _enemies.Add(newBossGalaga);

            _bossGalagaNextPos.X += Constants.CHARACTER_DIMENSIONS;
            if (_bossGalagaNextPos.X > Constants.GAMEPLAY_X)
            {
                _bossGalagaNextPos.X = 50;
                _bossGalagaNextPos.Y += Constants.CHARACTER_DIMENSIONS;
            }



            /*            _createdEnemies++;
                        EnemyBee newBee = new(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                            _beeTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
                        {
                            EntrancePath = _points.ToList(),
                            Destination = _beeNextPos
                        };
                        _enemies.Add(newBee);

                        _beeNextPos.X += Constants.CHARACTER_DIMENSIONS;
                        if (_beeNextPos.X > Constants.GAMEPLAY_X)
                        {
                            _beeNextPos.X = 50;
                            _beeNextPos.Y += Constants.CHARACTER_DIMENSIONS;
                        }*/

            /*            _createdEnemies++;
                        EnemyButterfly newButterfly = new(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                        _butterflyTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
                        {
                            EntrancePath = _points.ToList(),
                            Destination = _butterflyNextPos
                        };
                        _enemies.Add(newButterfly);
                        _butterflyNextPos.X += Constants.CHARACTER_DIMENSIONS;
                        if (_butterflyNextPos.X > Constants.GAMEPLAY_X)
                        {
                            _butterflyNextPos.X = 50;
                            _butterflyNextPos.Y += Constants.CHARACTER_DIMENSIONS;
                        }*/
                        _elapsedTime -= _entranceDelay;
        }

        foreach (Enemy enemy in _enemies)
        {
            enemy.Update(gameTime.ElapsedGameTime);
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        foreach(Enemy enemy in _enemies)
            enemy.Render(spriteBatch);
    }

    public override void ObjectHit(Guid id)
    {
        Enemy deadEnemy = _enemies.First(e => e.Id == id);
        _particleSystem.EnemyDeath(deadEnemy.Position);
        _enemies.Remove(deadEnemy);
    }
}