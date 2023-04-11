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

    public PlayerShip(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture, int numberOfSubImages) 
        : base(position, dimensions, texture, 10_000, debugTexture, numberOfSubImages)
    {
        _bounds = bounds;
    }

    protected override void Translate(Point offset)
    {
        if (Position.X + offset.X > 0 && Position.X + offset.X < 210)
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