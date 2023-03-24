using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Galaga.Systems;

public class AudioSystem : System
{
    private bool _playingMusic;
    private readonly Song _song;

    public AudioSystem(Song song)
    {
        _song = song;
    }
    public void Stop()
    {
        if (!_playingMusic) return;
        MediaPlayer.Stop();
    }
    
    public override void Update(GameTime gameTime)
    {
        if (_playingMusic) return;
        
        _playingMusic = true;
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_song);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
    }
}