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
    private readonly Dictionary<PlayStates, SubPlayState> _playStates = new();
    private AudioSystem _audioSystem;

    private void InitializeState()
    {
        _playStates.Clear();
        _playStates.Add(PlayStates.Loser, new LoserState(Graphics, Window));
        _playStates.Add(PlayStates.Play, new PlaySubPlayState(Graphics, Window, Textures));
        _playStates.Add(PlayStates.Pause, new PauseSubPlayState(Graphics, Window));
       
        _currentPlayState = PlayStates.Play;
        _nextPlayState = PlayStates.Play;
    } 
    
    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));
        Fonts.Add("vBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
        //Song song = contentManager.Load<Song>("Audio/Take Me Out to the Ball Game");
        _audioSystem = new AudioSystem(null);
        Textures.Add("ship", contentManager.Load<Texture2D>("Images/PlayerShip"));
        Textures.Add("playerBullet", contentManager.Load<Texture2D>("Images/PlayerBullet"));
        Textures.Add("enemyBullet", contentManager.Load<Texture2D>("Images/EnemyBullet"));
        Textures.Add("debug", contentManager.Load<Texture2D>("Images/Debug"));
        Textures.Add("particle", contentManager.Load<Texture2D>("Images/Particle"));
        Textures.Add("bee", contentManager.Load<Texture2D>("Images/Bee"));
        Textures.Add("butterfly", contentManager.Load<Texture2D>("Images/Butterfly"));
        Textures.Add("bossGalagaFull", contentManager.Load<Texture2D>("Images/BossGalagaFull"));
        Textures.Add("bossGalagaHalf", contentManager.Load<Texture2D>("Images/BossGalagaHalf"));
        Textures.Add("dragonfly", contentManager.Load<Texture2D>("Images/Dragonfly"));

        Textures.Add("background", contentManager.Load<Texture2D>("Images/Background"));
        InitializeState();
    }

    public override GameStates Update(GameTime gameTime)
    {
        _nextPlayState = _playStates[_currentPlayState].Update(gameTime);
        if (_nextPlayState != PlayStates.Finish) return GameStates.GamePlay;
        
        _audioSystem.Stop();
        InitializeState();
        return GameStates.MainMenu;
    }

    public override void Render()
    {
        //SpriteBatch.Begin();
        _playStates[_currentPlayState].Render(SpriteBatch, Fonts);
        //SpriteBatch.End();
        _currentPlayState = _nextPlayState;
    }
}