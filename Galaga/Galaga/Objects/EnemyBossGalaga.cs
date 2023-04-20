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
    public class EnemyBossGalaga : Enemy
    {
        float amplitude = 25f; // amplitude value for vertical movement
        float frequency = 0.03f; // frequency value for horizontal movement

        private Point _playerPosition;
        private List<Vector2> _path;

        private List<Vector2> _rotatedPath;

        private Point _startAttackPos;

        private bool isStartingOnLeft;

        private List<Texture2D> _textures;


        public EnemyBossGalaga(Point position, Point dimensions, List<Texture2D> textures, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack)
        : base(position, dimensions, textures, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack)
        {
            _path = new();
            _rotatedPath = new();
            isStartingOnLeft = true;
            health = 2;
            _textures = textures;
        }

        protected override void Attack()
        {
            isStartingOnLeft = Position.X < Constants.GAMEPLAY_X / 2;
            _playerPosition = Player.Position;
            _startAttackPos = Position;

            if (isStartingOnLeft)
            {
                // Create Counter clockwise circle path, then travel on sin wave path

                // Counter Clockwise path
                _path.AddRange(CircleCreator.CreateCounterClockwiseCircle(_startAttackPos.X + 24, _startAttackPos.Y + 24, 24));

                // Sin wave path
                _path.AddRange(CircleCreator.CreateSinWavePath(amplitude, frequency, 200f, _path[_path.Count-1].X, 500f, _path[_path.Count - 1].Y, 5f));
            }
            else
            {
                // Create clockwise circle path, then travel on sin wave path

                // Clockwise circle path
                _path.AddRange(CircleCreator.CreateClockwiseCircle(Position.X + 24, Position.Y - 24, 24));


                // Sin wave path
                _path.AddRange(CircleCreator.CreateSinWavePath(amplitude, frequency, 125f, _path[_path.Count - 1].X, 500f, _path[_path.Count - 1].Y, 5f));
            }

            float angleInRadians = CircleCreator.GetAngleRadians(new(Position.X, Position.Y), new(_playerPosition.X, _playerPosition.Y));
            _rotatedPath = CircleCreator.RotatePath(_path, angleInRadians, Position.X, Position.Y);
            //_rotatedPath = _path;

            base.Attack();
        }

        private void ResetVelocity()
        {
            VelocityX = 0;
            VelocityY = 0;
        }
        public override void CalculateAttackPath()
        {
            // Set the velocity x and y to be for the path of attack
            base.CalculateAttackPath();

            // Calculate velocity to go along the path
            if (_rotatedPath.Count > 0)
            {
                Vector2 nextPoint = _rotatedPath[0];
                
                float xDistance = nextPoint.X - Position.X;
                float xDistanceSquared = xDistance * xDistance;
                float yDistance = nextPoint.Y - Position.Y;
                float yDistanceSquared = yDistance * yDistance;
                double totalDistance = Math.Sqrt(xDistanceSquared + yDistanceSquared);
                if (totalDistance <= Constants.CHARACTER_DIMENSIONS)
                    _rotatedPath.RemoveAt(0);
                VelocityX = VelocityVector * xDistance / totalDistance;
                VelocityY = VelocityVector * yDistance / totalDistance;
            }
            // If the butterfly hits the bottom of the stage, then wrap
            if (Position.Y > Constants.GAMEPLAY_Y + 50)
            {
                _rotatedPath.Clear();
                _path.Clear();
                ResetVelocity();
                Position.X = _startAttackPos.X;
                Position.Y = _startAttackPos.Y - 150;
                VelocityY = VelocityVector / 2;
            }
            // Set the velocity to be straight down after the butterfly hits the bottom of the stage and it teleported up.
            if (Position.Y >= _startAttackPos.Y && _rotatedPath.Count == 0 && _path.Count == 0)
            {
                VelocityY = 0;
                Position.Y = _startAttackPos.Y;
                ResetAttackTimer();
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            if (health <= 1)
            {
                ObjectTexture = _textures[1];   
            }
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);

            if (DEBUG)
            {
                for (int i = 0; i < _rotatedPath.Count; i++)
                {
                    Vector2 point = _rotatedPath[i];
                    spriteBatch.Draw(_debugTexture, new Vector2(point.X, point.Y), Color.White);
                }
            }
        }

    }
}
