using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class EnemyBee : Enemy
{
    private List<Vector2> _path;


    private Point _startAttackPos;
    private bool _isStartingOnLeft;
    public EnemyBee(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack) 
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack)
    {
        _path = new();
        health = 1;
        _isStartingOnLeft = true;
    }

    protected override void Attack()
    {
        _isStartingOnLeft = Position.X < Constants.GAMEPLAY_X / 2;
        _startAttackPos = Position;

        if (_isStartingOnLeft)
        {
            List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(_startAttackPos.X - 24, _startAttackPos.Y - 8, 24).ToList();
            topCounterClockwiseSemiCircle.RemoveRange(20, 20);
            _path.AddRange(topCounterClockwiseSemiCircle);
            _path.Add(new(2*Constants.GAMEPLAY_X/3, Constants.GAMEPLAY_Y/2));
            _path.Add(new(2 * Constants.GAMEPLAY_X / 3, 7 * Constants.GAMEPLAY_Y / 8));
            _path.AddRange(CircleCreator.CreateClockwiseSemiCircle(2 * Constants.GAMEPLAY_X / 3 - 32, 7 * Constants.GAMEPLAY_Y / 8, 32));
            _path.Add(new(_startAttackPos.X, _startAttackPos.Y));
        }
        else
        {
            List<Vector2> topClockwiseCircle = CircleCreator.CreateClockwiseCircle(_startAttackPos.X + 24, _startAttackPos.Y - 8, 24).ToList();
            topClockwiseCircle.RemoveRange(20, 20);
            _path.AddRange(topClockwiseCircle);
            _path.Add(new(Constants.GAMEPLAY_X / 3, Constants.GAMEPLAY_Y / 2));
            _path.Add(new(Constants.GAMEPLAY_X / 3, 7 * Constants.GAMEPLAY_Y / 8));
            _path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 3 + 32, 7 * Constants.GAMEPLAY_Y / 8, 32));
            _path.Add(new(_startAttackPos.X, _startAttackPos.Y));
        }


        //_path.AddRange(CircleCreator.CreateSinWavePath(amplitude, frequency, 0f, Position.X, 600f, Position.Y, 2f));
/*        float angleInRadians = CircleCreator.GetAngleRadians(new(Position.X, Position.Y), new(_playerPosition.X, _playerPosition.Y));
        _rotatedPath = CircleCreator.RotatePath(_path, angleInRadians, Position.X, Position.Y);*/
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
        else
        {
            _path.Clear();
            ResetVelocity();
            ResetAttackTimer();
        }
/*        // If the bee hits the bottom of the stage, then wrap
        if (Position.Y > Constants.GAMEPLAY_Y)
        {
            _rotatedPath.Clear();
            _path.Clear();
            ResetVelocity();
            Position.X = _startAttackPos.X;
            Position.Y = _startAttackPos.Y - 150;
            VelocityY = VelocityVector / 2;
        }*/
        // Set the velocity to be straight down after the butterfly hits the bottom of the stage and it teleported up.
/*        if (Position.Y >= _startAttackPos.Y && _rotatedPath.Count == 0 && _path.Count == 0)
        {
            VelocityY = 0;
            Position.Y = _startAttackPos.Y;
            ResetAttackTimer();
        }*/
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        base.Render(spriteBatch);

        if (true)
        {
            for (int i = 0; i < _path.Count; i++)
            {
                Vector2 point = _path[i];
                spriteBatch.Draw(_debugTexture, new Vector2(point.X, point.Y), Color.White);
            }
        }
    }
}