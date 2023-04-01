using System.Collections.Generic;
using Galaga.States.SubPlayStates;
using Galaga.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Galaga.States;

public class PlayState : GameState
{
    private PlayStates _currentPlayState;
    private PlayStates _nextPlayState;
    private readonly Dictionary<PlayStates, SubPlayState> _playStates = new();
    private AudioSystem _audioSystem;

    private void InitializeState()
    {
        _playStates.Clear();
        _playStates.Add(PlayStates.Loser, new LoserState(Graphics, Window));
        _playStates.Add(PlayStates.Play, new PlaySubPlayState(Graphics, Window));
        _playStates.Add(PlayStates.Pause, new PauseSubPlayState(Graphics, Window));
       
        _currentPlayState = PlayStates.Play;
        _nextPlayState = PlayStates.Play;
    } 
    
    public override void LoadContent(ContentManager contentManager)
    {
        InitializeState();
        
        // var song = contentManager.Load<Song>("Audio/Take Me Out to the Ball Game");
        _audioSystem = new AudioSystem(null);
        foreach(var subState in _playStates)
            subState.Value.LoadContent(contentManager);
        
    }

    public override GameStates Update(GameTime gameTime)
    {
        ProcessInput();
        _nextPlayState = _playStates[_currentPlayState].Update(gameTime);
        if (_nextPlayState != PlayStates.Finish) return GameStates.GamePlay;
        
        _audioSystem.Stop();
        InitializeState();
        return GameStates.MainMenu;
    }

    public override void Render()
    {
        //SpriteBatch.Begin();
        _playStates[_currentPlayState].Render(SpriteBatch);
        //SpriteBatch.End();
        _currentPlayState = _nextPlayState;
    }

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }
}