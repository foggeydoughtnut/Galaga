using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NotImplementedException = System.NotImplementedException;

namespace Galaga.Systems;

public class AudioSystem
{
    private bool _playingMusic;
    private readonly IReadOnlyDictionary<string, SoundEffect> _soundEffects;
    private readonly Song _song;
    SoundEffectInstance _instance;
    List<SoundEffectInstance> _soundEffectInstances;

    public AudioSystem(Song song, IReadOnlyDictionary<string, SoundEffect> soundEffects)
    {
        _song = song;
        _soundEffects = soundEffects;
        _soundEffectInstances = new();
    }

    public void PlaySoundEffect(string name, float volume = -1)
    {
        if (volume == -1)
        {
            _instance = _soundEffects[name].CreateInstance();
            _instance.Play();
        }
        else
        {
            _instance = _soundEffects[name].CreateInstance();
            _instance.Volume = volume;
            _instance.Play();
        }
        _soundEffectInstances.Add(_instance);
    }

    public void StopSoundEffects()
    {
        foreach (SoundEffectInstance instance in _soundEffectInstances)
        {
            instance.Stop();
        }
    }

    public bool IsPlayingMusic()
    {
        return MediaPlayer.State == MediaState.Playing;
    }

    public void PlayStartup()
    {
        MediaPlayer.Play(_song);
        _playingMusic = true;
    }
    
    public void Stop()
    {
        if (!_playingMusic) return;
        MediaPlayer.Stop();
    }
}