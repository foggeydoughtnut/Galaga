using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Objects
{
    public class FakeEnemy : Object
    {
        private readonly Point _bounds;

        public FakeEnemy(Point position, Point bounds, Point dimensions, Texture2D texture, Texture2D debugTexture)
            : base(position, dimensions, texture, 10_000, debugTexture)
        {
            _bounds = bounds;
        }
    }
}
