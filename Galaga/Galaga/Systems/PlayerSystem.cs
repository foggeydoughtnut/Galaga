using System;
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
    public PlayerSystem(Texture2D shipTexture, GameStatsSystem gameStatsSystem, BulletSystem bulletSystem)
    {
        _bulletSystem = bulletSystem;
        _gameStatsSystem = gameStatsSystem;
        var shipDimensions = new Point(75, 75);
        _playerShip = new PlayerShip(new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - shipDimensions.Y),
            new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y), shipDimensions, shipTexture);
    }
    
    public override void Update(GameTime gameTime)
    {
        _playerShip.Update(gameTime.ElapsedGameTime);
        _playerShip.VelocityX = 0;
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
            _playerShip.VelocityX = 2500;
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            _playerShip.VelocityX = -2500;
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
            _bulletSystem.FirePlayerBullet(new Point(_playerShip.Position.X + _playerShip.Dimensions.X / 2, _playerShip.Position.Y));
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        _playerShip.Render(spriteBatch);
    }

    public override void ObjectHit(int id)
    {
    }
}