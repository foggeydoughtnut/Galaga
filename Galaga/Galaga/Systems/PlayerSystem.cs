using System;
using System.Collections.Generic;
using System.Diagnostics;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.Systems;

public class PlayerSystem : ObjectSystem
{
    private readonly BulletSystem _bulletSystem;
    private readonly GameStatsSystem _gameStatsSystem;
    private readonly PlayerShip _playerShip;
    private readonly ParticleSystem _particleSystem;

    private float speed = 7500f;


    public PlayerShip GetPlayer()
    {
        return _playerShip;
    }

    public PlayerSystem(Texture2D shipTexture, GameStatsSystem gameStatsSystem, BulletSystem bulletSystem, Texture2D debugTexture, ParticleSystem particleSystem)
    {
        _bulletSystem = bulletSystem;
        _gameStatsSystem = gameStatsSystem;
        _playerShip = new PlayerShip(new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - shipTexture.Height),
            new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y), new Point(shipTexture.Width, shipTexture.Height), shipTexture, debugTexture);
        _particleSystem = particleSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
        _playerShip.Update(gameTime.ElapsedGameTime);
        _playerShip.VelocityX = 0;
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
            _playerShip.VelocityX = speed * gameTime.ElapsedGameTime.TotalSeconds;
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            _playerShip.VelocityX = -speed * gameTime.ElapsedGameTime.TotalSeconds;
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
            _bulletSystem.FirePlayerBullet(new Point(_playerShip.Position.X + _playerShip.Dimensions.X / 2, _playerShip.Position.Y));
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        _playerShip.Render(spriteBatch);
    }

    public override void ObjectHit(Guid id)
    {
    }
    public void PlayerHit()
    {
        _particleSystem.PlayerDeath(_playerShip.Position);

        //Debug.WriteLine("Player was hit");
    }
}