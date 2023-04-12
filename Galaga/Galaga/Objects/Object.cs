using System;
using System.Collections.Generic;
using System.Diagnostics;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public abstract class Object
{
    const bool DEBUG_COLLIDER = false;

    public bool IsObstacle = true;
    public Guid Id;
    public Point Position;
    public readonly Point Dimensions;
    public double VelocityX;
    public double VelocityY;
    private double _totalElapsedMicrosecondsX;
    private double _totalElapsedMicrosecondsY;
    protected readonly Texture2D Texture;
    private readonly int _numAnimations;
    private readonly Point _subImageDimensions;
    private readonly Texture2D DebugTexture;
    private int _currentTextureIndex;
    private readonly TimeSpan _animationTime;
    private TimeSpan _elapsedAnimationTime;
    public Rectangle Collider => new(Position, Dimensions);

    protected Object(Point position, Point dimensions, Texture2D texture, int numAnimations, int animationTimeMilliseconds, Point subImageDimensions,
        Texture2D debugTexture)
    {
        Position = position;
        Dimensions = dimensions;
        Texture = texture;
        DebugTexture = debugTexture;
        _subImageDimensions = subImageDimensions;
        _numAnimations = numAnimations;

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
            if (_currentTextureIndex >= _numAnimations)
                _currentTextureIndex = 0;
        }

        UpdatePosition(elapsedTime);
    }

    public double DetermineHeading()
    {
        if (VelocityX == 0 && VelocityY == 0)
            return 0;
        var heading = Math.Atan2(VelocityY, VelocityX);
        return heading  + Constants.PI_OVER_TWO;
    }
    
    public virtual void Render(SpriteBatch spriteBatch)
    {
        if (Texture is null) return;
        spriteBatch.Draw(Texture,
            new Rectangle(Position.X + Dimensions.X / 2, Position.Y + Dimensions.Y / 2, Dimensions.X, Dimensions.Y),
            new Rectangle(_currentTextureIndex * _subImageDimensions.X, 0, _subImageDimensions.X, _subImageDimensions.Y),
            Color.White,
            (float)DetermineHeading(),
            new Vector2(_subImageDimensions.X / 2, _subImageDimensions.Y / 2),
            SpriteEffects.None,
            0);

        if (DEBUG_COLLIDER)
        {
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.X + Collider.Width/2, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.X + Collider.Width/2, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Right, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Left, Collider.Top), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Right, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
            spriteBatch.Draw(DebugTexture, new Vector2(Collider.Left, Collider.Bottom), null, Color.Green, 0f, new Vector2(DebugTexture.Width / 2, DebugTexture.Height / 2), 0.5f, SpriteEffects.None, 0.25f);
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