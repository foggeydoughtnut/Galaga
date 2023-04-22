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

    public AudioSystem(Song song, IReadOnlyDictionary<string, SoundEffect> soundEffects)
    {
        _song = song;
        _soundEffects = soundEffects;
    }

    public void PlaySoundEffect(string name)
    {
        _soundEffects[name].Play();
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