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
    private readonly List<Texture2D> _bossGalagaTextures;

    private readonly Texture2D _debugTexture;
    private readonly int _maxEnemies = 10;
    private int _createdEnemies;

    private bool _isBonusRound;

    private readonly Texture2D _dragonflyTexture;
    private List<Vector2> _dragonflyPathOdd;
    private List<Vector2> _dragonflyPathEven;

    private readonly Texture2D _satelliteTexture;
    private List<Vector2> _satellitePath;


    public IEnumerable<Enemy> GetEnemies() => _enemies.ToList();

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem, ParticleSystem particleSystem, GameWindow window, Texture2D beeTexture, Texture2D debugTexture, Texture2D butterflyTexture, List<Texture2D> bossGalagaTextures, Texture2D dragonflyTexture, Texture2D satelliteTexture)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _particleSystem = particleSystem;
        _beeTexture = beeTexture;
        _debugTexture = debugTexture;
        _butterflyTexture = butterflyTexture;
        _bossGalagaTextures = bossGalagaTextures;
        _dragonflyTexture = dragonflyTexture;
        _satelliteTexture = satelliteTexture;
        _dragonflyPathOdd = new();
        _dragonflyPathEven = new();
        _satellitePath = new();


        _enemies = new List<Enemy>();
        _window = window;
        _bossGalagaNextPos = new Vector2(50.0f, 18.0f);
        _butterflyNextPos = new Vector2(50.0f, 50.0f);
        _beeNextPos = new Vector2(50.0f, 82.0f);

        _points = new List<Vector2> { new(200, 0) };
        Random rand = new();
        int randX = rand.Next() % (Constants.GAMEPLAY_X / 2) + Constants.GAMEPLAY_X / 4;
        int randY = rand.Next() % (Constants.GAMEPLAY_Y / 4) + Constants.GAMEPLAY_Y / 4;
        _points.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(randX, randY, EntranceCircleRadius));

        _isBonusRound = false;
    }

    private void GenerateDragonflyPath(bool oddEnemy)
    {
        if (oddEnemy)
        {
            _dragonflyPathOdd.Add(new(Constants.GAMEPLAY_X/2, Constants.GAMEPLAY_Y/2));
            _dragonflyPathOdd.Add(new(Constants.GAMEPLAY_X / 5, 3 * Constants.GAMEPLAY_Y / 5));
            _dragonflyPathOdd.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2));
            _dragonflyPathOdd.Add(new(Constants.GAMEPLAY_X / 2, 0));
        }
        else
        {
            _dragonflyPathEven.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2));
            _dragonflyPathEven.Add(new(4 * Constants.GAMEPLAY_X / 5, 3 * Constants.GAMEPLAY_Y / 5));
            _dragonflyPathEven.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2));
            _dragonflyPathEven.Add(new(Constants.GAMEPLAY_X / 2, 0));
        }
    }

    private void GenerateSatellitePath()
    {
        _satellitePath.Add(new(Constants.GAMEPLAY_X / 4 - 48, Constants.GAMEPLAY_Y / 2));
        _satellitePath.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2, 48));
        _satellitePath.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X/4, Constants.GAMEPLAY_Y/2, 48));
        _satellitePath.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2, 32));
        _satellitePath.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2, 16));
        _satellitePath.Add(new(-50, Constants.GAMEPLAY_Y/2));
    }
    
    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemies)
        {
            _createdEnemies++;
            EnemyBossGalaga newBossGalaga = new(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                _bossGalagaTextures, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
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

            if (_isBonusRound)
            {
                #region Dragonfly
                _createdEnemies++;
                /*if (_dragonflyPathOdd.Count > 0 && _dragonflyPathEven.Count > 0) // Potentially alternate the ways by checking if the dragonfly count is odd or even
                {
                    if (_createdEnemies % 2 == 0)
                    {
                        EnemyDragonfly newDragonfly = new(new Point(Constants.GAMEPLAY_X / 2, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                            _dragonflyTexture, 99999, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
                        {
                            EntrancePath = _dragonflyPathEven.ToList(), // If it is odd it should have different path then when its even
                            Destination = new Vector2(_dragonflyPathEven[_dragonflyPathEven.Count - 1].X, _dragonflyPathEven[_dragonflyPathEven.Count - 1].Y - Constants.GAMEPLAY_Y)
                        };
                        _enemies.Add(newDragonfly);
                    }
                    else
                    {
                        EnemyDragonfly newDragonfly = new(new Point(Constants.GAMEPLAY_X/2, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                            _dragonflyTexture, 99999, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
                        {
                            EntrancePath = _dragonflyPathOdd.ToList(), // If it is odd it should have different path then when its even
                            Destination = new Vector2(_dragonflyPathOdd[_dragonflyPathOdd.Count-1].X, _dragonflyPathOdd[_dragonflyPathOdd.Count - 1].Y - Constants.GAMEPLAY_Y)
                        };
                        _enemies.Add(newDragonfly);
                    }
                }
                else
                {
                    GenerateDragonflyPath(false);
                    GenerateDragonflyPath(true);
                }*/
                #endregion

                #region Satellite

                if (_satellitePath.Count > 0)
                {
                    EnemySatellite newSatellite = new(new Point(Constants.GAMEPLAY_X / 2, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                        _satelliteTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem)
                        {
                            EntrancePath = _satellitePath.ToList(),
                            Destination = new Vector2(_satellitePath[^1].X, _satellitePath[^1].Y - Constants.GAMEPLAY_Y)
                        };
                    _enemies.Add(newSatellite);
                }
                else
                {
                    GenerateSatellitePath();
                }
                #endregion
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
        Enemy hitEnemy = _enemies.First(e => e.Id == id);
        hitEnemy.health--;
        if (hitEnemy.health <= 0 )
        {
            _particleSystem.EnemyDeath(hitEnemy.Position);
            _enemies.Remove(hitEnemy);
        }
    }
}