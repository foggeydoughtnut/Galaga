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


public class WaveEnemy
{
    public string Type;
    public List<Vector2> Path;
    public Point StartPos;


    public WaveEnemy(string type, List<Vector2> path, Point startPos)
    {
        Type = type;
        Path = path;
        StartPos = startPos;
    }
}

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
    private readonly int _maxEnemiesPerRound = 40;
    private int _createdEnemies;


    private readonly Texture2D _dragonflyTexture;
    private List<Vector2> _dragonflyPathOdd;
    private List<Vector2> _dragonflyPathEven;

    private readonly Texture2D _satelliteTexture;
    private List<Vector2> _satellitePath;

    #region Rounds
    private bool _isBonusRound;
    private int _waveNumber;
    private int _roundCount;
    List<List<WaveEnemy>> _roundOneEnemies; // This holds a list of lists of tuples (sorry haha) They are a list of enemy groups. Each enemy group has a list of tuples with enemy type, their entrance path, and start pos
    List<List<Tuple<string, List<Vector2>, Point>>> _roundTwoEnemies;
    List<List<Tuple<string, List<Vector2>, Point>>> _bonusRoundOneEnemies;
    private int _numOfGroupsPerStage = 5;
    private int _numberOfEnemiesPerSubGroup = 4; // There are two subgroups per wave
    private int _groupIndex;
    private int _enemyIndex;
    private bool _roundFinished;

    private float _groupTimer;
    private bool _groupTimerActive;
    private float _groupDelay;
    #endregion


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


        #region Rounds Initialization
        _isBonusRound = false;
        _roundOneEnemies = new();
        AddRoundOne();
        _roundTwoEnemies = new();
        _bonusRoundOneEnemies = new();
        _groupIndex = 0;
        _enemyIndex = 0;
        _roundFinished = false;

        _groupTimer = 0f;
        _groupTimerActive = false;
        _groupDelay = 3f;

        #endregion
    }

    private void AddRoundOne()
    {
        for (int i = 0; i < _numOfGroupsPerStage; i++)
        {
            List<WaveEnemy> group = new();
            for (int firstGroup = 0; firstGroup < _numberOfEnemiesPerSubGroup; firstGroup++) // Create first subgroup of enemies
            {
                WaveEnemy enemy;
                if (i == 0) // First group
                {
                    enemy = new("bee", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X/2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X/2, 0));
                    group.Add(enemy);
                }
                else if (i == 1)
                {
                    enemy = new("bossGalaga", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else if (i == 2)
                {
                    enemy = new("butterfly", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else if (i == 3)
                {
                    enemy = new("bee", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else
                {
                    enemy = new("bee", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                
            }
            for (int secondGroup = 0; secondGroup < _numberOfEnemiesPerSubGroup; secondGroup++) // Second subgroup of enemies
            {
                WaveEnemy enemy;
                if (i == 0) // First group
                {
                    enemy = new("butterfly", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else if (i == 1)
                {
                    enemy = new("butterfly", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else if (i == 2)
                {
                    enemy = new("butterfly", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else if (i == 3)
                {
                    enemy = new("bee", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
                else
                {
                    enemy = new("bee", CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4, EntranceCircleRadius).ToList(), new(Constants.GAMEPLAY_X / 2, 0));
                    group.Add(enemy);
                }
            }
            _roundOneEnemies.Add(group);
        }
    }

    #region Bonus Round Paths
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
    #endregion

    public override void Update(GameTime gameTime)
    {
        if (_groupTimerActive)
        {
            _groupTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_groupTimer >= _groupDelay)
            {
                _groupTimerActive = false;
                _groupTimer = 0;
                _enemyIndex = 0;
                
            }
        }
 /*       foreach (List<WaveEnemy> group in _roundOneEnemies)
        {*/
        if (!_roundFinished && !_groupTimerActive && _groupIndex < _roundOneEnemies.Count)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            List<WaveEnemy> group = _roundOneEnemies[_groupIndex];
            if (_enemyIndex < group.Count)
            {
                if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemiesPerRound)
                {
                    WaveEnemy enemy = group[_enemyIndex];
                    _createdEnemies++;
                    _enemyIndex++;
                    if (enemy.Type == "bee")
                    {
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
                        }
                        // Spawn bee
                    }
                    else if (enemy.Type == "butterfly")
                    {
                        // Spawn butterfly
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
                        }

                    }
                    else
                    {
                        // Spawn boss galaga
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

                    }
                    _elapsedTime -= _entranceDelay;

                }
            }
            else
            {
                _groupTimerActive = true;
                _groupIndex++;
            }
            /*foreach(WaveEnemy enemy in group)
            {
                if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemiesPerRound)
                {

                    _createdEnemies++;
                    if (enemy.Type == "bee")
                    {
                        Debug.WriteLine("Spawn bee");
                        // Spawn bee
                    }
                    else if (enemy.Type == "butterfly")
                    {
                        // Spawn butterfly
                        Debug.WriteLine("Spawn butterfly");

                    }
                    else
                    {
                        // Spawn boss galaga
                        Debug.WriteLine("Spawn boss galaga");

                    }
                }
            }*/
        }

        //}
/*            EnemyBossGalaga newBossGalaga = new(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
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
            }*/

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