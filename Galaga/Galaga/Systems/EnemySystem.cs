using System;
using System.Collections.Generic;
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
    private const int EntranceCircleRadius = 150;
    private Vector2 _nextPos;
    private List<Vector2> _points;
    private readonly TimeSpan _entranceDelay = new(0,0,0,0,500);
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private readonly Texture2D _beeTexture;
    private readonly Texture2D _debugTexture;
    private readonly int _maxEnemies = 10;
    private int _createdEnemies;

    public IEnumerable<Enemy> GetEnemies() => _enemies.ToList();

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem, ParticleSystem particleSystem, GameWindow window, Texture2D beeTexture, Texture2D debugTexture)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _particleSystem = particleSystem;
        _beeTexture = beeTexture;
        _debugTexture = debugTexture;
        _enemies = new List<Enemy>();
        _window = window;
        _nextPos = new Vector2(50.0f, 50.0f);
        _points = new List<Vector2> { new(200, 0) };
        var rand = new Random();
        var randX = rand.Next() % (Constants.GAMEPLAY_X / 2) + Constants.GAMEPLAY_X / 4;
        var randY = rand.Next() % (Constants.GAMEPLAY_Y / 4) + Constants.GAMEPLAY_Y / 2;
        _points.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(randX, randY, EntranceCircleRadius));
    }

    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime > _entranceDelay && _createdEnemies < _maxEnemies)
        {
            _createdEnemies++;
            var newBee = new EnemyBee(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),
                _beeTexture, 1000, _debugTexture)
            {
                EntrancePath = _points.ToList(),
                Destination = _nextPos
            };
            _enemies.Add(newBee);
            _elapsedTime -= _entranceDelay;
            _nextPos.X += Constants.CHARACTER_DIMENSIONS;
            if (_nextPos.X > Constants.GAMEPLAY_X)
            {
                _nextPos.X = 50;
                _nextPos.Y += Constants.CHARACTER_DIMENSIONS;
            }
        }
        
        foreach(var enemy in _enemies)
            enemy.Update(gameTime.ElapsedGameTime);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        foreach(var enemy in _enemies)
            enemy.Render(spriteBatch);
    }

    public override void ObjectHit(Guid id)
    {
        var deadEnemy = _enemies.First(e => e.Id == id);
        _particleSystem.EnemyDeath(deadEnemy.Position);
        _enemies.Remove(deadEnemy);
    }
}