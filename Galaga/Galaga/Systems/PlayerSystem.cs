using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.Systems;

public class PlayerSystem : ObjectSystem
{
    private readonly BulletSystem _bulletSystem;
    private readonly GameStatsSystem _gameStatsSystem;
    private PlayerShip _playerShip;

    private float speed = 7500f;

    public PlayerSystem(GameStatsSystem gameStatsSystem, BulletSystem bulletSystem)
    {
        _bulletSystem = bulletSystem;
        _gameStatsSystem = gameStatsSystem;
    }

    public override void LoadContent(ContentManager contentManager)
    {
        var shipTexture = contentManager.Load<Texture2D>("Images/PlayerShip");
        var debugTexture = contentManager.Load<Texture2D>("Images/Debug");
        _playerShip = new PlayerShip(new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - Constants.CHARACTER_DIMENSIONS),
            new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y), 
            new Point(Constants.CHARACTER_DIMENSIONS), 
            shipTexture, debugTexture);
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

    public override void ObjectHit(int id)
    {
    }
}