using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Objects
{
    public class EnemyDragonfly : Enemy
    {

        public EnemyDragonfly(Point position, Point dimensions, Texture2D textures, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack, AudioSystem audioSystem)
        : base(position, dimensions, textures, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack, audioSystem)
        {
            health = 1;
        }
      


        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);
/*
            if (DEBUG)
            {
                for (int i = 0; i < _rotatedPath.Count; i++)
                {
                    Vector2 point = _rotatedPath[i];
                    spriteBatch.Draw(_debugTexture, new Vector2(point.X, point.Y), Color.White);
                }
            }*/
        }

    }
}

