using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Objects
{
    public class EnemyButterfly : Enemy
    {
        float amplitude = 25f; // amplitude value for vertical movement
        float frequency = 0.05f; // frequency value for horizontal movement
        private bool _isStartingOnLeft;

        private Point _playerPosition;
        private List<Vector2> _path;



        public EnemyButterfly(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack)
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack)
        {
            _path = new();
            health = 1;
            _isStartingOnLeft = true;

        }

        protected override void Attack()
        {
            StartAttackPos = Position;
            _isStartingOnLeft = Position.X < Constants.GAMEPLAY_X / 2;

            _playerPosition = Player.Position;

            if (_isStartingOnLeft)
            {
                List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(StartAttackPos.X - 16, StartAttackPos.Y - 8, 16).ToList();
                topCounterClockwiseSemiCircle.RemoveRange(28, 12);
                _path.AddRange(topCounterClockwiseSemiCircle);
            }
            else
            {
                List<Vector2> topClockwiseCircle = CircleCreator.CreateClockwiseCircle(StartAttackPos.X + 16, StartAttackPos.Y - 8, 16).ToList();
                topClockwiseCircle.RemoveRange(28, 12);
                _path.AddRange(topClockwiseCircle);
            }

            List<Vector2> sinWave = CircleCreator.CreateSinWavePath(amplitude, frequency, 0f, Position.X + 24, 600f, Position.Y, 2f).ToList();
            float angleInRadians = CircleCreator.GetAngleRadians(new(Position.X, Position.Y), new(_playerPosition.X, _playerPosition.Y));
            sinWave = CircleCreator.RotatePath(sinWave, angleInRadians, Position.X, Position.Y);
            _path.AddRange(sinWave);
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
            if (_path.Count > 0)
            {
                Vector2 nextPoint = _path[0];
                float xDistance = nextPoint.X - Position.X;
                float xDistanceSquared = xDistance * xDistance;
                float yDistance = nextPoint.Y - Position.Y;
                float yDistanceSquared = yDistance * yDistance;
                double totalDistance = Math.Sqrt(xDistanceSquared + yDistanceSquared);
                if (totalDistance <= Constants.CHARACTER_DIMENSIONS)
                    _path.RemoveAt(0);
                VelocityX = VelocityVector * xDistance / totalDistance;
                VelocityY = VelocityVector * yDistance / totalDistance;
            }
            // If the butterfly hits the bottom of the stage, then wrap
            if (Position.Y > Constants.GAMEPLAY_Y)
            {
                _path.Clear();
                Position.X = StartAttackPos.X;
                Position.Y = StartAttackPos.Y - 150;
                _path.Add(new(StartAttackPos.X, StartAttackPos.Y));
                VelocityY = VelocityVector/2;
            }
            // Set the velocity to be straight down after the butterfly hits the bottom of the stage and it teleported up.
            if (Position.Y >= StartAttackPos.Y && _path.Count == 0)
            {
                VelocityY = 0;
                Position.Y = StartAttackPos.Y;
                Position.X = StartAttackPos.X;
                ResetVelocity();
                ResetAttackTimer();
            }
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);

            if (DEBUG)
            {
                for (int i = 0; i < _path.Count; i++)
                {
                    Vector2 point = _path[i];
                    spriteBatch.Draw(_debugTexture, new Vector2(point.X, point.Y), Color.White);
                }
            }
        }

    }
}
