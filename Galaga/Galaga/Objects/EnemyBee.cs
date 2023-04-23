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


    private bool _isStartingOnLeft;
    public EnemyBee(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack, AudioSystem audioSystem) 
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack, audioSystem)
    {
        _path = new();
        health = 1;
        _isStartingOnLeft = true;
    }

    protected override void Attack()
    {
        _isStartingOnLeft = Position.X < Constants.GAMEPLAY_X / 2;
        StartAttackPos = Position;

        if (_isStartingOnLeft)
        {
            List<Vector2> topCounterClockwiseSemiCircle = CircleCreator.CreateCounterClockwiseCircle(StartAttackPos.X - 24, StartAttackPos.Y - 8, 24).ToList();
            topCounterClockwiseSemiCircle.RemoveRange(28, 12);
            _path.AddRange(topCounterClockwiseSemiCircle);
            List<Vector2> cornerCircle = CircleCreator.CreateClockwiseCircle(2 * Constants.GAMEPLAY_X / 3 - 32, Constants.GAMEPLAY_Y / 2 + 32, 32).ToList();
            cornerCircle.RemoveRange(20, 20);
            cornerCircle.RemoveRange(0, 10);
            _path.AddRange(cornerCircle);
            _path.Add(new(2 * Constants.GAMEPLAY_X / 3, 7 * Constants.GAMEPLAY_Y / 8 - 16));
            _path.AddRange(CircleCreator.CreateClockwiseSemiCircle(2 * Constants.GAMEPLAY_X / 3 - 32, 7 * Constants.GAMEPLAY_Y / 8 - 16, 32));
            _path.Add(new(StartAttackPos.X, StartAttackPos.Y));
        }
        else
        {
            List<Vector2> topClockwiseCircle = CircleCreator.CreateClockwiseCircle(StartAttackPos.X + 24, StartAttackPos.Y - 8, 24).ToList();
            topClockwiseCircle.RemoveRange(28, 12);
            _path.AddRange(topClockwiseCircle);
            //_path.Add(new(Constants.GAMEPLAY_X / 3, Constants.GAMEPLAY_Y / 2));
            List<Vector2> cornerCircle = CircleCreator.CreateCounterClockwiseCircle(Constants.GAMEPLAY_X / 3 + 32, Constants.GAMEPLAY_Y / 2 + 32, 32).ToList();
            cornerCircle.RemoveRange(20, 20);
            cornerCircle.RemoveRange(0, 10);
            _path.AddRange(cornerCircle);
            _path.Add(new(Constants.GAMEPLAY_X / 3, 7 * Constants.GAMEPLAY_Y / 8 - 16));
            _path.AddRange(CircleCreator.CreateCounterClockwiseSemiCircle(Constants.GAMEPLAY_X / 3 + 32, 7 * Constants.GAMEPLAY_Y / 8 - 16, 32));
            _path.Add(new(StartAttackPos.X, StartAttackPos.Y));
        }

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
            if (totalDistance <= Constants.CHARACTER_DIMENSIONS/2)
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