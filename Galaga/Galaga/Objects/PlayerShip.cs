using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class PlayerShip : Object
{
    private readonly Point _bounds;
    public bool HasDuelShips = false;
    private const int RIGHT_WALL = 468;

    public PlayerShip(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture) 
        : base(position, dimensions, new List<Texture2D>{ texture }, 10_000, debugTexture)
    {
        _bounds = bounds;
    }

    protected override void Translate(Point offset)
    {
        //465ish
        if (Position.X + offset.X > 0 && Position.X + offset.X < RIGHT_WALL)
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
            spriteBatch.Draw(Textures[0], new Rectangle(new Point(Position.X - Dimensions.X, Position.Y), Dimensions), Color.White);
            spriteBatch.Draw(Textures[0], Collider, Color.White);
        }
    }
}