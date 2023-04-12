using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class PlayerShip : Object
{
    private readonly Point _bounds;
    public bool HasDuelShips = false;

    public PlayerShip(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture) 
        : base(position, dimensions, texture, 1, 10_000, new Point(15, 16),debugTexture)
    {
        _bounds = bounds;
    }

    protected override void Translate(Point offset)
    {
        //465ish
        if (Position.X + offset.X > 0 && Position.X + Dimensions.X + offset.X < _bounds.X)
        {
            Position.X += offset.X;
        }
        if (Position.Y + offset.Y > 0 && Position.Y + offset.Y < _bounds.Y - Dimensions.Y)
            Position.Y += offset.Y;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if(!HasDuelShips)
            base.Render(spriteBatch);
        else
        {
            spriteBatch.Draw(Texture, new Rectangle(new Point(Position.X - Dimensions.X, Position.Y), Dimensions), Color.White);
            spriteBatch.Draw(Texture, Collider, Color.White);
        }
    }
}