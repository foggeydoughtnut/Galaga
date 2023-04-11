using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class Bullet : Object
{
    public Bullet(Point position, Point dimensions, Texture2D texture, double velocity, Texture2D debugTexture) 
        : base(position, dimensions, texture, 10_000, debugTexture)
    {
        VelocityY = velocity;
    }
}