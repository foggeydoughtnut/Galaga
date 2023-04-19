using Galaga.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Objects
{
    public class EnemySatellite : Enemy
    {
        public EnemySatellite(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem)
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem)
        {
            health = 1;
        }
    }
}
