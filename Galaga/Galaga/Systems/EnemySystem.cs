using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Systems;


public class WaveEnemy
{
    public string Type;
    public List<Vector2> Path;
    public Point StartPos;
    public Vector2? Destination;

    public WaveEnemy(string type, List<Vector2> path, Point startPos, Vector2? destination=null)
    {
        Type = type;
        Path = path;
        StartPos = startPos;
        Destination = destination;
    }
}

public class EnemySystem : ObjectSystem
{
    private readonly PlayerSystem _playerSystem;
    private readonly BulletSystem _bulletSystem;
    private readonly ParticleSystem _particleSystem;
    private readonly AudioSystem _audioSystem;
    private readonly HighScoreTracker _scoreTracker;
    private readonly List<Enemy> _enemies;
    private readonly GameWindow _window;
    private Vector2 _butterflyNextPos;
    private Vector2 _beeNextPos;
    private Vector2 _bossGalagaNextPos;
    private readonly TimeSpan _entranceDelay = new(0,0,0,0,150);
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private readonly Texture2D _beeTexture;
    private readonly Texture2D _butterflyTexture;
    private readonly List<Texture2D> _bossGalagaTextures;

    private readonly Texture2D _debugTexture;
    private readonly int _maxEnemiesPerRound = 40;



    private int _createdEnemies;
    private int _destroyedEnemiesThisStage;
    public static int BonusRoundEnemiesDestroyed;

    #region bonus round enemies
    private readonly Texture2D _dragonflyTexture;
    private List<Vector2> _dragonflyPathOdd;
    private List<Vector2> _dragonflyPathEven;

    private readonly Texture2D _satelliteTexture;
    private List<Vector2> _satellitePath;
    #endregion

    #region Rounds
    private bool _isBonusRound;
    List<Tuple<List<WaveEnemy>, bool>> _roundOneEnemies; // List of groups of enemies
    List<Tuple<List<WaveEnemy>, bool>> _roundTwoEnemies;
    List<Tuple<List<WaveEnemy>, bool>> _bonusRoundOneEnemies;
    private List<List<Tuple<List<WaveEnemy>, bool>>> _rounds;
    private int _numOfGroupsPerStage = 5;
    private int _numberOfEnemiesPerGroup = 8;
    private int _enemyIndex;

    private int _groupIndex;
    private float _groupTimer;
    private bool _groupTimerActive;
    private float _groupDelay;
    private bool _stoppedRoundSoundEffects;
    private bool _roundFinished;
    private int _roundIndex;
    private float _roundTimer;
    private bool _roundTimerActive;
    private float _roundDelay;

    #endregion

    #region Breathing Variables
    private bool _breathDelayTimerActive;
    private float _breathDelayTimer;
    private float _breathDelay;
    public static bool Breathing;
    public static int BreathingDirection;
    #endregion

    #region In between stages variables
    public static bool DisplayStageNumber;
    public static int StageNumber;
    private bool _playedBonusJingle = false;
    private bool _playedStageJingle = false;
    private bool _playedBonusEndJingle = false;
    #endregion

    public static int HorizontalDirection;
    private Vector2 _idleMovementOffset;
    private Vector2 _idleVelocity;
    private double _totalElapsedTimeBreathing;
    public IEnumerable<Enemy> GetEnemies() => _enemies.ToList();


    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem, ParticleSystem particleSystem, GameWindow window, Texture2D beeTexture, Texture2D debugTexture, Texture2D butterflyTexture, List<Texture2D> bossGalagaTextures, Texture2D dragonflyTexture, Texture2D satelliteTexture, AudioSystem audioSystem)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _particleSystem = particleSystem;
        _scoreTracker = HighScoreTracker.GetTracker();
        _beeTexture = beeTexture;
        _debugTexture = debugTexture;
        _butterflyTexture = butterflyTexture;
        _bossGalagaTextures = bossGalagaTextures;
        _dragonflyTexture = dragonflyTexture;
        _satelliteTexture = satelliteTexture;
        _dragonflyPathOdd = new();
        _dragonflyPathEven = new();
        _satellitePath = new();
        _audioSystem = audioSystem;
        _idleMovementOffset = new();
        StageNumber = 2;
        DisplayStageNumber = false;

        _enemies = new List<Enemy>();
        _window = window;
        _bossGalagaNextPos = new Vector2(80.0f, 18.0f);
        _butterflyNextPos = new Vector2(80.0f, 50.0f);
        _beeNextPos = new Vector2(80.0f, 82.0f);

        _destroyedEnemiesThisStage = 0;

        #region Rounds Initialization
        _stoppedRoundSoundEffects = false;
        _isBonusRound = false;
        _roundOneEnemies = new();
        AddRoundOne();
        _roundTwoEnemies = new();
        AddRoundTwo();
        _bonusRoundOneEnemies = new();
        AddBonusRound();
        _groupIndex = 0;
        _enemyIndex = 0;

        _groupTimer = 0f;
        _groupTimerActive = false;
        _groupDelay = 3f;

        _rounds = new()
        {
            _roundOneEnemies,
            _roundTwoEnemies,
            _bonusRoundOneEnemies
        };

        _roundFinished = false;
        _roundIndex = 0;
        _roundTimerActive = false;
        _roundDelay = 5f;

        #endregion

        #region Breathing
        _breathDelayTimerActive = false;
        _breathDelayTimer = 0f;
        _breathDelay = 3f;
        Breathing = false;
        _totalElapsedTimeBreathing = 0;
        BreathingDirection = 1;
        #endregion
        HorizontalDirection = 1;

    }

    #region Entrance Paths
    private void AddRoundOne()
    {
        for (int i = 0; i < _numOfGroupsPerStage; i++)
        {
            Tuple<List<WaveEnemy>, bool> group;
            List<WaveEnemy> enemies = new();
            for (int j = 0; j < _numberOfEnemiesPerGroup; j++) // Create first subgroup of enemies
            {
                WaveEnemy enemy;
                if (i == 0) // First group
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 2 - 48, Constants.GAMEPLAY_Y / 3)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2, 48));
                        enemy = new("bee", path, new(Constants.GAMEPLAY_X/2 + 48, 0));
                        enemies.Add(enemy);

                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 2 + 48, Constants.GAMEPLAY_Y / 3)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2, 48));
                        enemy = new("butterfly", path, new(Constants.GAMEPLAY_X / 2 - 48, 0));

                        enemies.Add(enemy);
                    }
                }
                else if (i == 1)
                {
                    List<Vector2> path = new();
                    path.Add(new(Constants.GAMEPLAY_X / 4 + 24, Constants.GAMEPLAY_Y / 2));
                    path.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2, 24));
                    if (j%2 == 1)
                    {
                        enemy = new("bossGalaga", path, new(0, 7 * Constants.GAMEPLAY_Y / 8));
                    }
                    else
                    {
                        enemy = new("butterfly", path, new(0, 7 * Constants.GAMEPLAY_Y / 8));
                    }
                    enemies.Add(enemy);
                }
                else if (i == 2)
                {
                    List<Vector2> path = new()
                    {
                        new(3 * Constants.GAMEPLAY_X / 4 - 24, Constants.GAMEPLAY_Y / 2)
                    };
                    path.AddRange(CircleCreator.CreateClockwiseCircle(3 * Constants.GAMEPLAY_X / 4 + 36 - 24, Constants.GAMEPLAY_Y / 2, 36));
                    enemy = new("butterfly", path, new(Constants.GAMEPLAY_X, 7 * Constants.GAMEPLAY_Y / 8));
                    enemies.Add(enemy);
                }
                else if (i == 3)
                {
                    List<Vector2> path = new()
                    {
                        new(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2)
                    };
                    path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 4 + 24, Constants.GAMEPLAY_Y / 2 + 16, 24));

                    enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0));
                    enemies.Add(enemy);
                }
                else
                {
                    List<Vector2> path = new()
                    {
                        new(3 * Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2)
                    };
                    path.AddRange(CircleCreator.CreateClockwiseSemiCircle(3 * Constants.GAMEPLAY_X / 4 - 24, Constants.GAMEPLAY_Y / 2 + 16, 24));

                    enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0));
                    enemies.Add(enemy);
                }
            }
            if (i == 0)
                group = new(enemies, true);
            else
                group = new(enemies, false);

            _roundOneEnemies.Add(group);
        }
    }

    private void AddRoundTwo()
    {
        for (int i = 0; i < _numOfGroupsPerStage; i++)
        {
            Tuple<List<WaveEnemy>, bool> group;
            List<WaveEnemy> enemies = new();
            for (int j = 0; j < _numberOfEnemiesPerGroup; j++) // Create first subgroup of enemies
            {
                WaveEnemy enemy;
                if (i == 0) // First group
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 2 - 48, Constants.GAMEPLAY_Y / 3)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2 - 48, Constants.GAMEPLAY_Y / 2, 24));
                        path.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4));

                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2 + 48, 0));
                        enemies.Add(enemy);
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 2 + 48, Constants.GAMEPLAY_Y / 3)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseSemiCircle(Constants.GAMEPLAY_X / 2 + 48, Constants.GAMEPLAY_Y / 2, 24));
                        path.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 4));
                        enemy = new("butterfly", path, new(Constants.GAMEPLAY_X / 2 - 48, 0));

                        enemies.Add(enemy);
                    }
                }
                else if (i == 1)
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 4 - 8, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 4 - 32, Constants.GAMEPLAY_Y / 2, 24));
                        enemy = new("bossGalaga", path, new(0, 7 * Constants.GAMEPLAY_Y / 8));
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 4 - Constants.CHARACTER_DIMENSIONS - 8, Constants.GAMEPLAY_Y / 2 - Constants.CHARACTER_DIMENSIONS/2)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 4 - 32 - Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2 - Constants.CHARACTER_DIMENSIONS/2, 24));
                        enemy = new("butterfly", path, new(-Constants.CHARACTER_DIMENSIONS, 7 * Constants.GAMEPLAY_Y / 8 - Constants.CHARACTER_DIMENSIONS/2));
                    }
                    enemies.Add(enemy);
                }
                else if (i == 2)
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(3 * Constants.GAMEPLAY_X / 4 - 8, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseCircle(3 * Constants.GAMEPLAY_X / 4 + 16, Constants.GAMEPLAY_Y / 2, 24));
                        enemy = new("butterfly", path, new(Constants.GAMEPLAY_X, 7 * Constants.GAMEPLAY_Y / 8));
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(3 * Constants.GAMEPLAY_X / 4 + Constants.CHARACTER_DIMENSIONS - 8, Constants.GAMEPLAY_Y/2 - Constants.CHARACTER_DIMENSIONS/2)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseCircle(3 * Constants.GAMEPLAY_X / 4 + 16 + Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2 - Constants.CHARACTER_DIMENSIONS/2, 24));
                        enemy = new("butterfly", path, new(Constants.GAMEPLAY_X + Constants.CHARACTER_DIMENSIONS, 7 * Constants.GAMEPLAY_Y / 8 - Constants.CHARACTER_DIMENSIONS/2));
                    }
                    enemies.Add(enemy);
                }
                else if (i == 3)
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 4 + 24, Constants.GAMEPLAY_Y / 2 + 16, 24));
                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0));
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(Constants.GAMEPLAY_X / 4 + Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 4 + 24 + Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2 + 16, 24));
                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2 + Constants.CHARACTER_DIMENSIONS, 0));
                    }

                    enemies.Add(enemy);
                }
                else
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new()
                        {
                            new(3 * Constants.GAMEPLAY_X / 4, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseSemiCircle(3 * Constants.GAMEPLAY_X / 4 - 24, Constants.GAMEPLAY_Y / 2 + 16, 24));

                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0));
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            new(3 * Constants.GAMEPLAY_X / 4 - Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2)
                        };
                        path.AddRange(CircleCreator.CreateClockwiseSemiCircle(3 * Constants.GAMEPLAY_X / 4 - 24 - Constants.CHARACTER_DIMENSIONS, Constants.GAMEPLAY_Y / 2 + 16, 24));

                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2 - Constants.CHARACTER_DIMENSIONS, 0));
                    }
                    enemies.Add(enemy);
                }
            }
            group = new(enemies, true);
            _roundTwoEnemies.Add(group);
        }
    }

    private void AddBonusRound()
    {
        for (int i = 0; i < _numOfGroupsPerStage; i++)
        {
            Tuple<List<WaveEnemy>, bool> group;
            List<WaveEnemy> enemies = new();
            for (int j = 0; j < _numberOfEnemiesPerGroup; j++) // Create first subgroup of enemies
            {
                WaveEnemy enemy;
                if (i == 0) // First group
                {
                    if (j % 2 == 1)
                    {
                        List<Vector2> path = new();
           
                        path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, 3 * Constants.GAMEPLAY_Y / 5, 48));
                        path.Add(new(Constants.GAMEPLAY_X + 50, Constants.GAMEPLAY_Y / 2));

                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2 + 48, 0), new(Constants.GAMEPLAY_X + 50, Constants.GAMEPLAY_Y / 2));
                        enemies.Add(enemy);
                    }
                    else
                    {
                        List<Vector2> path = new();
 
                        path.AddRange(CircleCreator.CreateClockwiseSemiCircle(Constants.GAMEPLAY_X / 2, 3 * Constants.GAMEPLAY_Y / 5, 48));
                        path.Add(new(-50, Constants.GAMEPLAY_Y / 2));
                        enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2 - 48, 0), new(-50, Constants.GAMEPLAY_Y / 2));
                        enemies.Add(enemy);
                    }
                }
                else if (i == 1)
                {
                    List<Vector2> path = new()
                    {
                        new(Constants.GAMEPLAY_X / 2 + 32, 3 * Constants.GAMEPLAY_Y / 4 - 24),
                        new(Constants.GAMEPLAY_X / 2 + 32, Constants.GAMEPLAY_Y / 4 - 24),
                    };

                    List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 2 + 16, Constants.GAMEPLAY_Y / 4 - 24, 16).ToList();
                    topCounterClockwiseSemiCircle.RemoveRange(20, 20);
                    path.AddRange(topCounterClockwiseSemiCircle);

                    path.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2 - 24));

                    List<Vector2> bottomCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 2 + 16, Constants.GAMEPLAY_Y / 2 - 24, 16).ToList();
                    bottomCounterClockwiseSemiCircle.RemoveRange(0, 20);
                    path.AddRange(bottomCounterClockwiseSemiCircle);

                    if (j % 2 == 1)
                    {
                        enemy = new("bossGalaga", path, new(0, 7 * Constants.GAMEPLAY_Y / 8 - 24), new(Constants.GAMEPLAY_X + 50, -50));
                    }
                    else
                    {
                        enemy = new("bee", path, new(0, 7 * Constants.GAMEPLAY_Y / 8 - 24), new(Constants.GAMEPLAY_X + 50, -50));
                    }
                    enemies.Add(enemy);
                }
                else if (i == 2)
                {
                    List<Vector2> path = new()
                    {
                        new(Constants.GAMEPLAY_X / 2 - 32, 3 * Constants.GAMEPLAY_Y / 4 - 24),
                        new(Constants.GAMEPLAY_X / 2 - 32, Constants.GAMEPLAY_Y / 4 - 24),
                    };

                    List<Vector2> topClockwiseSemiCircle = CircleCreator.CreateClockwiseCircle(Constants.GAMEPLAY_X / 2 - 16, Constants.GAMEPLAY_Y / 4 - 24, 16).ToList();
                    topClockwiseSemiCircle.RemoveRange(20, 20);
                    path.AddRange(topClockwiseSemiCircle);

                    path.Add(new(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y / 2 - 24));

                    List<Vector2> bottomClockwiseSemiCircle = CircleCreator.CreateClockwiseCircle(Constants.GAMEPLAY_X / 2 - 16, Constants.GAMEPLAY_Y / 2 - 24, 16).ToList();
                    bottomClockwiseSemiCircle.RemoveRange(0, 20);
                    path.AddRange(bottomClockwiseSemiCircle);

                    enemy = new("bee", path, new(7 * Constants.GAMEPLAY_X / 8, 7 * Constants.GAMEPLAY_Y / 8 - 24), new(-50, -50));
                    enemies.Add(enemy);
                }
                else if (i == 3)
                {
                    List<Vector2> path = new();
                    path.AddRange(CircleCreator.CreateClockwiseSemiCircle(Constants.GAMEPLAY_X / 4 + 36, 3 * Constants.GAMEPLAY_Y / 4 - 36, 36));

                    List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateClockwiseCircle(Constants.GAMEPLAY_X / 4 + 36, 3 * Constants.GAMEPLAY_Y / 4 - 36, 36).ToList();
                    topCounterClockwiseSemiCircle.RemoveRange(10, 30);
                    path.AddRange(topCounterClockwiseSemiCircle);

                    enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0), new(Constants.GAMEPLAY_X + 50, Constants.GAMEPLAY_Y / 4));
                    enemies.Add(enemy);
                }
                else
                {
                    List<Vector2> path = new();
                    path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(3 * Constants.GAMEPLAY_X / 4 - 36, 3 * Constants.GAMEPLAY_Y / 4 - 36, 36));

                    List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(3 * Constants.GAMEPLAY_X / 4 - 36, 3 * Constants.GAMEPLAY_Y / 4 - 36, 36).ToList();
                    topCounterClockwiseSemiCircle.RemoveRange(10, 30);
                    path.AddRange(topCounterClockwiseSemiCircle);

                    enemy = new("bee", path, new(Constants.GAMEPLAY_X / 2, 0), new(-50, Constants.GAMEPLAY_Y / 4));
                    enemies.Add(enemy);
                }
            }
            if (i == 0)
                group = new(enemies, true);
            else
                group = new(enemies, false);

            _bonusRoundOneEnemies.Add(group);
        }
    }
    #endregion

    #region Bonus Round Paths NOT NEEDED ENEMIES
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
        if (Math.Sin(0.5 * Math.PI * gameTime.TotalGameTime.TotalSeconds) >= 0)
            HorizontalDirection = 1;
        else
            HorizontalDirection = -1;
        if (Breathing)
        {
            _totalElapsedTimeBreathing += gameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Sin(0.5 * Math.PI * _totalElapsedTimeBreathing) >= 0)
                BreathingDirection = 1;
            else
                BreathingDirection = -1;
        }


        _idleVelocity = new(HorizontalDirection, 0);
        _idleVelocity *= 90;
        _idleVelocity *= (float)gameTime.ElapsedGameTime.TotalSeconds;
        _idleMovementOffset += _idleVelocity;

        if (Math.Abs(_idleMovementOffset.X) > 4)
        {
            _bossGalagaNextPos.X += _idleMovementOffset.X < 0 ? -1 : 1;
            _butterflyNextPos.X += _idleMovementOffset.X < 0 ? -1 : 1;
            _beeNextPos.X += _idleMovementOffset.X < 0 ? -1 : 1;

            _idleMovementOffset.X = 0;
        }



        if (_groupTimerActive)
        {
            if (_isBonusRound) _groupDelay = 4.5f;
            else _groupDelay = 3f;
            _groupTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_groupTimer >= _groupDelay)
            {
                _groupTimerActive = false;
                _groupTimer = 0;
                _enemyIndex = 0;
                
            }
        }
        if (_roundTimerActive && _roundTimer > 1.5f)
        {
            DisplayStageNumber = true;
            if (StageNumber % 3 == 0)
            {
                if (!_playedBonusJingle)
                {
                    _audioSystem.PlaySoundEffect("bonus");
                    _playedBonusJingle = true;
                }
            }
            else
            {
                if (!_playedBonusEndJingle && _isBonusRound)
                {
                    _audioSystem.PlaySoundEffect("bonusEnd");
                    _playedBonusEndJingle = true;
                }
                
                if (!_playedStageJingle && _roundDelay - _roundTimer < 2.5f)
                {
                    _audioSystem.PlaySoundEffect("stage");
                    _playedStageJingle = true;
                }
            }
            

        }
        if (_roundTimerActive)
        {
            if (_roundTimer > 1f)
            {
                if (!_stoppedRoundSoundEffects)
                {
                    _audioSystem.StopSoundEffects();
                    _stoppedRoundSoundEffects = true;
                }
            }
            if (_isBonusRound) _roundDelay = 10f;
            else _roundDelay = 5f;
            _roundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_roundTimer >= _roundDelay)
            {
                _destroyedEnemiesThisStage = 0;
                _roundTimerActive = false;
                _roundTimer = 0;
                _enemyIndex = 0;
                DisplayStageNumber = false;
                _roundIndex++;
                StageNumber++;
                _groupIndex = 0;
                _groupTimerActive = false;
                _enemyIndex = 0;
                _roundFinished = false;
                _createdEnemies = 0;
                Breathing = false;
                _stoppedRoundSoundEffects = false;
                _playedBonusJingle = false;
                _playedStageJingle = false;
                _playedBonusEndJingle = false;

                if (_roundIndex % 2 == 0) // bonus round
                {
                    _isBonusRound = true;
                }
                else
                {
                    _bossGalagaNextPos = new Vector2(80.0f, 34.0f);
                    _butterflyNextPos = new Vector2(80.0f, 66.0f);
                    _beeNextPos = new Vector2(80.0f, 98.0f);
                }
                if (_roundIndex % _rounds.Count == 0)
                {
                    _roundIndex = 0; // Make it just loop
                    _isBonusRound = false;
                }

            }
        }
        if (_breathDelayTimerActive)
        {
            _breathDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_breathDelayTimer > _breathDelay)
            {
                if (!Breathing && !_isBonusRound)
                    _audioSystem.PlaySoundEffect("enemyBreathing", 0.25f);
                Breathing = true;
                _breathDelayTimerActive = false;
            }
        }
        if (_destroyedEnemiesThisStage >= _maxEnemiesPerRound)
        {
            _roundFinished = true;
            _roundTimerActive = true;
            Breathing = false;
            _breathDelayTimerActive = false;
            _breathDelayTimer = 0f;
        }

        #region Remove any enemies that are off the screen during bonus round
        if (_isBonusRound)
        {
            List<Enemy> enemiesToRemove = new();
            foreach (Enemy enemy in _enemies)
            {
                if (enemy.Position.X > Constants.GAMEPLAY_X || enemy.Position.X < 0)
                {
                    enemiesToRemove.Add(enemy);
                }
            }
            foreach (Enemy enemy in enemiesToRemove)
            {
                _enemies.Remove(enemy);
                _destroyedEnemiesThisStage++;
            }
        }
        #endregion

        if (!_roundFinished && !_groupTimerActive && _groupIndex < _numOfGroupsPerStage) // Spawn enemies in the group
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            Tuple<List<WaveEnemy>, bool> group = _rounds[_roundIndex][_groupIndex];

            if (_enemyIndex < group.Item1.Count)
            {
                if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemiesPerRound)
                {
                    List<WaveEnemy> enemiesToSpawn = new();
                    WaveEnemy _enemy = group.Item1[_enemyIndex];
                    enemiesToSpawn.Add(_enemy);
                    WaveEnemy _secondEnemy = null;
                    if (group.Item2)
                    {
                        _secondEnemy = group.Item1[_enemyIndex + 1];
                        enemiesToSpawn.Add(_secondEnemy);
                        _createdEnemies++;
                        _enemyIndex++;
                    }


                    _createdEnemies++;
                    _enemyIndex++;
                    foreach(WaveEnemy enemy in enemiesToSpawn)
                    {
                        if (enemy.Type == "bee")
                        {
                            // Spawn bee
                            EnemyBee newBee = new(enemy.StartPos, new Point(Constants.CHARACTER_DIMENSIONS),
                                _beeTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem, !_isBonusRound, _audioSystem)
                                {
                                    EntrancePath = enemy.Path.ToList(),
                                    Destination = !_isBonusRound ? _beeNextPos : enemy.Destination
                                };
                            _enemies.Add(newBee);

                            _beeNextPos.X += Constants.CHARACTER_DIMENSIONS;
                            if (_beeNextPos.X > Constants.GAMEPLAY_X - 80f)
                            {
                                _beeNextPos.X = 80f;
                                _beeNextPos.Y += Constants.CHARACTER_DIMENSIONS;
                            }
                        }
                        else if (enemy.Type == "butterfly")
                        {
                            // Spawn butterfly
                            EnemyButterfly newButterfly = new(enemy.StartPos, new Point(Constants.CHARACTER_DIMENSIONS),
                                _butterflyTexture, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem, !_isBonusRound, _audioSystem)
                                {
                                    EntrancePath = enemy.Path.ToList(),
                                    Destination = !_isBonusRound ? _butterflyNextPos : enemy.Destination

                            };
                            _enemies.Add(newButterfly);
                            _butterflyNextPos.X += Constants.CHARACTER_DIMENSIONS;
                            if (_butterflyNextPos.X > Constants.GAMEPLAY_X - 80f)
                            {
                                _butterflyNextPos.X = 80f;
                                _butterflyNextPos.Y += Constants.CHARACTER_DIMENSIONS;
                            }

                        }
                        else
                        {
                            // Spawn boss galaga
                            EnemyBossGalaga newBossGalaga = new(enemy.StartPos, new Point(Constants.CHARACTER_DIMENSIONS),
                                _bossGalagaTextures, 1000, _debugTexture, _playerSystem.GetPlayer(), _bulletSystem, !_isBonusRound, _audioSystem)
                                {
                                    EntrancePath = enemy.Path.ToList(),
                                    Destination = !_isBonusRound ? _bossGalagaNextPos : enemy.Destination
                                };
                            _enemies.Add(newBossGalaga);

                            _bossGalagaNextPos.X += Constants.CHARACTER_DIMENSIONS;
                            if (_bossGalagaNextPos.X > Constants.GAMEPLAY_X - 80f)
                            {
                                _bossGalagaNextPos.X = 80.0f;
                                _bossGalagaNextPos.Y += Constants.CHARACTER_DIMENSIONS;
                            }

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
            

            if (_createdEnemies == _maxEnemiesPerRound && !_breathDelayTimerActive)
            {
                _breathDelayTimerActive = true;
            }
        }

/*            if (_isBonusRound)
            {
                #region Dragonfly
                _createdEnemies++;
                *//*if (_dragonflyPathOdd.Count > 0 && _dragonflyPathEven.Count > 0) // Potentially alternate the ways by checking if the dragonfly count is odd or even
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
                }*//*
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

    public void ObjectHit(Guid id, bool instantDeath = false)
    {
        Enemy hitEnemy = _enemies.First(e => e.Id == id);
        hitEnemy.health--;

        if (hitEnemy.health > 0 && !instantDeath)
        {
            _audioSystem.PlaySoundEffect("boss.1");
            return;
        }

        _destroyedEnemiesThisStage++;
        if (_isBonusRound)
            BonusRoundEnemiesDestroyed++;
        else
            BonusRoundEnemiesDestroyed = 0;
        _particleSystem.EnemyDeath(hitEnemy.Position);
        _enemies.Remove(hitEnemy);
        ScoreDestroyedEnemy(hitEnemy);
        switch (hitEnemy)
        {
            case EnemyBee:
                _audioSystem.PlaySoundEffect("bee");
                break;
            case EnemyButterfly:
                _audioSystem.PlaySoundEffect("butterfly");
                break;                
            case EnemyBossGalaga:
                _audioSystem.PlaySoundEffect("boss.2");
                break;                
        }
    }

    private void ScoreDestroyedEnemy(Enemy enemy)
    {
        switch (enemy)
        {
            case EnemyBee:
                _scoreTracker.CurrentGameScore += enemy.attack ? 100 : 50;
                break;
            case EnemyButterfly:
                _scoreTracker.CurrentGameScore += enemy.attack ? 160 : 80;
                break;
            case EnemyBossGalaga boss:
                if (boss.attack)
                    _scoreTracker.CurrentGameScore += boss.NumEscorts switch
                    {
                        2 => 1600,
                        1 => 800,
                        _ => 400
                    };
                else
                    _scoreTracker.CurrentGameScore += 150;
                break;
        }
    }
}