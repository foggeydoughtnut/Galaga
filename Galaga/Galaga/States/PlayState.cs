using System.Collections.Generic;
using Galaga.States.SubPlayStates;
using Galaga.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Galaga.States;

public class PlayState : GameState
{
    private PlayStates _currentPlayState;
    private PlayStates _nextPlayState;
    private readonly Dictionary<PlayStates, ISubPlayState> _playStates = new();
    private List<Systems.System> _systems;
    private AudioSystem _audioSystem;

    public PlayState()
    {
        _systems = new List<Systems.System>();
    }
    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        base.Initialize(graphicsDevice, graphics);
        RunStartup();
    }

    private void RunStartup()
    {
        var gameStats = new GameStatsSystem();
        var bulletSystem = new BulletSystem(gameStats);
        var playerSystem = new PlayerSystem(gameStats, bulletSystem);
        var enemySystem = new EnemySystem(playerSystem, bulletSystem);
        var collisionDetectionSystem = new CollisionDetectionSystem(playerSystem, enemySystem, bulletSystem);
        _systems.Add(bulletSystem);
        _systems.Add(enemySystem);
        _systems.Add(bulletSystem);
        _systems.Add(collisionDetectionSystem);

        _playStates.Clear();
        _playStates.Add(PlayStates.Loser, new LoserState());
        _playStates.Add(PlayStates.Play, new PlaySubPlayState());
        _playStates.Add(PlayStates.Startup, new StartupSubPlayState());
        _playStates.Add(PlayStates.Pause, new PauseSubPlayState());
       
        _currentPlayState = PlayStates.Startup;
    } 
    
    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));
        Fonts.Add("vBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
        // var song = contentManager.Load<Song>("Audio/Take Me Out to the Ball Game");
        _audioSystem = new AudioSystem(null);
    }

    public override GameStates Update(GameTime gameTime)
    {
        if (_currentPlayState == PlayStates.Startup)
            RunStartup();

        ProcessInput();
        _nextPlayState = _playStates[_currentPlayState].Update(gameTime);
        if (_nextPlayState == PlayStates.Startup)
        {
            _audioSystem.Stop();
            return GameStates.MainMenu;
        }
        return GameStates.GamePlay;
    }

    public override void Render(GameTime gameTime)
    {
        SpriteBatch.Begin();
        _playStates[_currentPlayState].Render(SpriteBatch);
        SpriteBatch.End();
        _currentPlayState = _nextPlayState;
    }

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }
}