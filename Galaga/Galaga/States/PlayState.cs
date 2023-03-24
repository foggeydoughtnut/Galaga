using System.Collections.Generic;
using Galaga.States.SubPlayStates;
using Galaga.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.States;

public class PlayState : GameState
{
    private PlayStates _currentPlayState;
    private PlayStates _nextPlayState;
    private readonly Dictionary<PlayStates, SubPlayState> _playStates = new();
    private AudioSystem _audioSystem;

    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        base.Initialize(graphicsDevice, graphics);
        ResetState();
    }

    private void ResetState()
    {
        _playStates.Clear();
        _playStates.Add(PlayStates.Loser, new LoserState());
        _playStates.Add(PlayStates.Play, new PlaySubPlayState());
        _playStates.Add(PlayStates.Pause, new PauseSubPlayState());
       
        _currentPlayState = PlayStates.Play;
        _nextPlayState = PlayStates.Play;
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
        ProcessInput();
        _nextPlayState = _playStates[_currentPlayState].Update(gameTime);
        if (_nextPlayState != PlayStates.Finish) return GameStates.GamePlay;
        
        _audioSystem.Stop();
        ResetState();
        return GameStates.MainMenu;
    }

    public override void Render()
    {
        SpriteBatch.Begin();
        _playStates[_currentPlayState].Render(SpriteBatch, Fonts);
        SpriteBatch.End();
        _currentPlayState = _nextPlayState;
    }

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }
}