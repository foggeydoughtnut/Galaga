using System;
using System.Collections.Generic;
using System.IO;
using Galaga.Objects;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private TimeSpan _playerLastShotTime;
    private TimeSpan _playerShotDelay = TimeSpan.FromSeconds(0.25);
    private KeyboardState _previousKeyboardState;
    private readonly AudioSystem _audioSystem;
    public bool PlayerKilled;
    private int _lives;


    public PlayerShip GetPlayer()
    {
        return _playerShip;
    }

    public PlayerSystem(Texture2D shipTexture, GameStatsSystem gameStatsSystem, BulletSystem bulletSystem, Texture2D debugTexture, ParticleSystem particleSystem, AudioSystem audioSystem)
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
        _audioSystem = audioSystem;

        _playerShip = new PlayerShip(
            position: new Point(Constants.GAMEPLAY_X / 2, Constants.GAMEPLAY_Y - Constants.CHARACTER_DIMENSIONS),
            bounds: new Point(Constants.GAMEPLAY_X, Constants.GAMEPLAY_Y),
            dimensions: new Point(Constants.CHARACTER_DIMENSIONS),
            shipTexture,
            debugTexture,
            numberOfSubImages: 1
        );
        _previousKeyboardState = Keyboard.GetState();
    }

    public override void Update(GameTime gameTime)
    {
        string controlsFileData = File.ReadAllText("controls.json");
        _controls = JsonConvert.DeserializeObject<Controls>(controlsFileData);
        //Debug.WriteLine(controls.Right.ToString());
        _playerShip.Update(gameTime.ElapsedGameTime);
        var currentKeyboardState = Keyboard.GetState();
        if (currentKeyboardState.IsKeyDown(_controls.Right))
            _playerShip.MovePlayer(gameTime.ElapsedGameTime, 1);
        if (currentKeyboardState.IsKeyDown(_controls.Left))
            _playerShip.MovePlayer(gameTime.ElapsedGameTime, -1);
        if (currentKeyboardState.IsKeyUp(_controls.Fire) && _previousKeyboardState.IsKeyDown(_controls.Fire))
        {
            _bulletSystem.FirePlayerBullet(new Point(_playerShip.Position.X + _playerShip.Dimensions.X / 2, _playerShip.Position.Y));
            _playerLastShotTime = gameTime.TotalGameTime;
        }

        _previousKeyboardState = currentKeyboardState;
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
        _soundEffects["death"].Play();
        PlayerKilled = true;
        //Debug.WriteLine("Player was hit");
    }
}