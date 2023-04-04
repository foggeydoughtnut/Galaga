using System;
using System.Collections.Generic;
using System.Diagnostics;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public abstract class Object
{
    const bool DEBUG_COLLIDER = true;

    public bool IsObstacle = true;
    public Guid Id;
    public Point Position;
    public readonly Point Dimensions;
    public double VelocityX;
    public double VelocityY;
    private double _totalElapsedMicrosecondsX;
    private double _totalElapsedMicrosecondsY;
    protected readonly List<Texture2D> Textures;
    private readonly Texture2D DebugTexture;
    private int _currentTextureIndex;
    private readonly TimeSpan _animationTime;
    private TimeSpan _elapsedAnimationTime;
    public Rectangle Collider => new(Position, Dimensions);

    protected Object(Point position, Point dimensions, List<Texture2D> textures, int animationTimeMilliseconds, Texture2D debugTexture)
    {
        Position = position;
        Dimensions = dimensions;
        Textures = textures;
        DebugTexture = debugTexture;

        _animationTime = new TimeSpan(0,0,0,0, animationTimeMilliseconds);
        Id = Guid.NewGuid();
    }

    public virtual void Update(TimeSpan elapsedTime)
    {
        _elapsedAnimationTime += elapsedTime;
        if (_elapsedAnimationTime > _animationTime)
        {
            _elapsedAnimationTime -= _animationTime;
            _currentTextureIndex++;
            if (_currentTextureIndex >= Textures.Count)
                _currentTextureIndex = 0;
        }

        UpdatePosition(elapsedTime);
    }

    public virtual void Render(SpriteBatch spriteBatch)
    {
        if (Textures is null) return;
        spriteBatch.Draw(Textures[_currentTextureIndex], Collider, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        if (DEBUG_COLLIDER)
        {
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.X + Collider.Width/2, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.X + Collider.Width/2, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Right, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Left, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Right, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Left, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.5f);
        }
    }

    private void UpdatePosition(TimeSpan elapsedTime)
    {
        var movementX = 0;
        var movementY = 0;

        if (VelocityX != 0)
        {
            _totalElapsedMicrosecondsX += elapsedTime.Milliseconds * 1000;
            var microsecondsToMoveX = Convert.ToInt32(1_000_000 / Math.Abs(VelocityX));
            var pixelsToMoveX = Convert.ToInt32(Math.Floor(_totalElapsedMicrosecondsX / microsecondsToMoveX));
            _totalElapsedMicrosecondsX -= pixelsToMoveX * microsecondsToMoveX;
            movementX = pixelsToMoveX * (VelocityX < 0 ? -1 : 1);
        }

        if (VelocityY != 0)
        {
            _totalElapsedMicrosecondsY += elapsedTime.Milliseconds * 1000;
            var microsecondsToMoveY = Convert.ToInt32(1_000_000 / Math.Abs(VelocityY));
            var pixelsToMoveY = Convert.ToInt32(Math.Floor(_totalElapsedMicrosecondsY / microsecondsToMoveY));
            _totalElapsedMicrosecondsY -= pixelsToMoveY * microsecondsToMoveY;
            movementY = pixelsToMoveY * (VelocityY < 0 ? -1 : 1);
        }
        
        Translate(new Point(movementX, movementY));
    }

    protected virtual void Translate(Point offset)
    {
        Position.X += offset.X;
        Position.Y += offset.Y;
    }
}