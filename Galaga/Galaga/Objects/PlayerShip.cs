using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class PlayerShip : Object
{
    private readonly Point _bounds;
    public bool HasDuelShips = false;
    private float _speed = 7500f;

/*    public PlayerShip(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture, int numberOfSubImages)
        : base(position, dimensions, texture, 500, debugTexture, numberOfSubImages)
    {
        _bounds = bounds;
    } // DELETE THIS*/


    public PlayerShip(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture, int numberOfSubImages)
    : base(position, dimensions, texture, 10_000, debugTexture, numberOfSubImages)
    {
        _bounds = bounds;
        CannotRotate = true;
    }

    public override void Update(TimeSpan elapsedTime)
    {
        base.Update(elapsedTime);
        VelocityX = 0;
    }

    public void MovePlayer(TimeSpan elapsedTime, int direction)
    {
        VelocityX = _speed * elapsedTime.TotalSeconds * direction;
    }
    
    protected override void Translate(Point offset)
    {
        //465ish
        if (Position.X + offset.X > 0 && Position.X + Dimensions.X + offset.X < _bounds.X)
        {
            Position.X += offset.X;
        }
        if (Position.Y + offset.Y > 0 && Position.Y + offset.Y < Constants.GAMEPLAY_Y - ObjectTexture.Height)
            Position.Y += offset.Y;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if(!HasDuelShips)
            base.Render(spriteBatch);
        else
        {
            spriteBatch.Draw(ObjectTexture, new Rectangle(new Point(Position.X - Dimensions.X, Position.Y), Dimensions), Color.White);
            spriteBatch.Draw(ObjectTexture, Collider, Color.White);
        }
    }
}