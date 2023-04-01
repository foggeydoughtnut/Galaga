using System;
using System.Collections.Generic;
using Galaga.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Object = Galaga.Objects.Object;

namespace Galaga.Systems;

public class EnemySystem : ObjectSystem
{
    private readonly PlayerSystem _playerSystem;
    private readonly BulletSystem _bulletSystem;
    private readonly List<Object> _enemies;

    public EnemySystem(PlayerSystem playerSystem, BulletSystem bulletSystem)
    {
        _playerSystem = playerSystem;
        _bulletSystem = bulletSystem;
        _enemies = new List<Object>();
    }

    public override void LoadContent(ContentManager contentManager)
    {
        var beeTexture = contentManager.Load<Texture2D>("Images/Bee");
        var debugTexture = contentManager.Load<Texture2D>("Images/Debug");
        _enemies.Add(new EnemyBee(new Point(100, 100), new Point(beeTexture.Width / 2, beeTexture.Height),beeTexture, 1000, debugTexture));
    }

    public override void Update(GameTime gameTime)
    {
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