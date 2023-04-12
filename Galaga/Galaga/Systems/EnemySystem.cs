using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Object = Galaga.Objects.Object;

namespace Galaga.Systems;

public class EnemySystem : ObjectSystem
{
    private readonly PlayerSystem _playerSystem;
    private readonly BulletSystem _bulletSystem;
    private readonly List<Enemy> _enemies;
    private readonly GameWindow _window;
    private const int EntranceCircleRadius = 150;
    private Vector2 _nextPos;
    private List<Vector2> _points;
    private bool _sentCircle;
    private readonly TimeSpan _entranceDelay = new(0,0,0,0,200);
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private Texture2D _beeTexture;
    private Texture2D _debugTexture;
    

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem, GameWindow window)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _enemies = new List<Enemy>();
        _window = window;
        _nextPos = new Vector2(50, 50);
        _points = new List<Vector2> { new(200, 0) };
        var rand = new Random();
        var randX = rand.Next() % (Constants.GAMEPLAY_X / 2) + Constants.GAMEPLAY_X / 4;
        var randY = rand.Next() % (Constants.GAMEPLAY_Y / 4) + Constants.GAMEPLAY_Y / 2;
        _points.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(randX, randY, EntranceCircleRadius));
    }

    public override void LoadContent(ContentManager contentManager)
    {
        _beeTexture = contentManager.Load<Texture2D>("Images/Bee");
        _debugTexture = contentManager.Load<Texture2D>("Images/Debug");
        _enemies.Add(new EnemyBee(new Point(210, 0), new Point(Constants.CHARACTER_DIMENSIONS),_beeTexture, 1000, _debugTexture));
    }

    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime > _entranceDelay)
        {
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

    public override void ObjectHit(int id)
    {
    }
}