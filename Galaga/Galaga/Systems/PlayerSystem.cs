using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Galaga.Systems;

public class PlayerSystem : ObjectSystem
{
    private readonly BulletSystem _bulletSystem;
    private readonly GameStatsSystem _gameStatsSystem;
    private readonly PlayerShip _playerShip;
    private readonly ParticleSystem _particleSystem;
    private Controls _controls;

    private float speed = 7500f;

    public PlayerShip GetPlayer()
    {
        return _playerShip;
    }

    public PlayerSystem(Texture2D shipTexture, GameStatsSystem gameStatsSystem, BulletSystem bulletSystem, Texture2D debugTexture, ParticleSystem particleSystem)
    {
        if (Controls.ChangeInControls)
        {
            if (!File.Exists("controls.json"))
            {
                Controls newControls = new Controls { Left = Keys.Left, Right = Keys.Right, Fire = Keys.Space };
                string json = JsonConvert.SerializeObject(newControls);
                File.WriteAllText("controls.json", json);
            }

            string controlsFileData = File.ReadAllText("controls.json");
            _controls = JsonConvert.DeserializeObject<Controls>(controlsFileData);
        }
        else
        {
            Controls.ChangeInControls = false;
        }

        _bulletSystem = bulletSystem;
        _gameStatsSystem = gameStatsSystem;
        _particleSystem = particleSystem;


        _playerShip = new PlayerShip(
            position: new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - Constants.CHARACTER_DIMENSIONS),
            bounds: new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y),
            dimensions: new Point(Constants.CHARACTER_DIMENSIONS),
            shipTexture,
            debugTexture,
            numberOfSubImages: 1
        );

        // THIS IS FOR TESTING ANIMATION
        /*        _playerShip = new PlayerShip(
                    position: new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - shipTexture.Height),
                    bounds: new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y),
                    dimensions: new Point(shipTexture.Width / 2, shipTexture.Height),
                    shipTexture,
                    debugTexture,
                    numberOfSubImages: 2
                );*/

    }

    public override void Update(GameTime gameTime)
    {
        string controlsFileData = File.ReadAllText("controls.json");
        _controls = JsonConvert.DeserializeObject<Controls>(controlsFileData);
        //Debug.WriteLine(controls.Right.ToString());
        _playerShip.Update(gameTime.ElapsedGameTime);
        _playerShip.VelocityX = 0;
        if (Keyboard.GetState().IsKeyDown(_controls.Right))
            _playerShip.VelocityX = speed * gameTime.ElapsedGameTime.TotalSeconds;
        if (Keyboard.GetState().IsKeyDown(_controls.Left))
            _playerShip.VelocityX = -speed * gameTime.ElapsedGameTime.TotalSeconds;
        if (Keyboard.GetState().IsKeyDown(_controls.Fire))
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