using System.Collections.Generic;
using Galaga.States.SubPlayStates;
using Galaga.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private bool _playedStartupEffect;

    private void InitializeState()
    {
        _playStates.Clear();
        _playStates.Add(PlayStates.Loser, new LoserState(Graphics, Window));
        _playStates.Add(PlayStates.Play, new PlaySubPlayState(Graphics, Window, Textures, SoundEffects));
        _playStates.Add(PlayStates.Pause, new PauseSubPlayState(Graphics, Window));
       
        _currentPlayState = PlayStates.Play;
        _nextPlayState = PlayStates.Play;
        _playedStartupEffect = false;
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
        Textures.Add("satellite", contentManager.Load<Texture2D>("Images/Satellite"));

        Textures.Add("background", contentManager.Load<Texture2D>("Images/Background"));

        SoundEffects.Add("start", contentManager.Load<SoundEffect>("Sound/Startup"));
        SoundEffects.Add("stage", contentManager.Load<SoundEffect>("Sound/StageFlag"));
        SoundEffects.Add("shot", contentManager.Load<SoundEffect>("Sound/Shooting"));
        SoundEffects.Add("enemyFly", contentManager.Load<SoundEffect>("Sound/EnemyFlying"));
        SoundEffects.Add("bee", contentManager.Load<SoundEffect>("Sound/Enemy1Death"));
        SoundEffects.Add("butterfly", contentManager.Load<SoundEffect>("Sound/Enemy2Death"));
        SoundEffects.Add("boss.1", contentManager.Load<SoundEffect>("Sound/Enemy3DeathPart1"));
        SoundEffects.Add("boss.2", contentManager.Load<SoundEffect>("Sound/Enemy3DeathPart2"));
        SoundEffects.Add("death", contentManager.Load<SoundEffect>("Sound/Death"));
        SoundEffects.Add("bonus", contentManager.Load<SoundEffect>("Sound/ChallengingStageStart"));
        SoundEffects.Add("bonusEnd", contentManager.Load<SoundEffect>("Sound/ChallengingStageResults"));
        InitializeState();
    }

    public override GameStates Update(GameTime gameTime)
    {
        if (!_playedStartupEffect)
        {
            _playedStartupEffect = true;
            SoundEffects["start"].Play();
            MediaPlayer.IsRepeating = false;
        }
        _nextPlayState = _playStates[_currentPlayState].Update(gameTime);
        if (_nextPlayState != PlayStates.Finish) return GameStates.GamePlay;
        
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