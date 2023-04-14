using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class EnemyBee : Enemy
{
    public EnemyBee(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture) 
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture)
    {
    }
}