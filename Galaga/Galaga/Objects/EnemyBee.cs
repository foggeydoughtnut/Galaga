using System;
using System.Collections.Generic;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public class EnemyBee : Enemy
{
    float amplitude = 25f; // amplitude value for vertical movement
    float frequency = 0.05f; // frequency value for horizontal movement

    private Point _playerPosition;
    private List<Vector2> _path;

    private List<Vector2> _rotatedPath;

    private Point _startAttackPos;
    public EnemyBee(Point position, Point dimensions, Texture2D texture, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack) 
        : base(position, dimensions, texture, 2, animationTimeMilliseconds, debugTexture, player, bulletSystem, canAttack)
    {
        _path = new();
        _rotatedPath = new();
        health = 1;
    }

    protected override void Attack()
    {
        _playerPosition = Player.Position;
        _startAttackPos = Position;
        _path.AddRange(CircleCreator.CreateSinWavePath(amplitude, frequency, 0f, Position.X, 600f, Position.Y, 2f));
        float angleInRadians = CircleCreator.GetAngleRadians(new(Position.X, Position.Y), new(_playerPosition.X, _playerPosition.Y));
        _rotatedPath = CircleCreator.RotatePath(_path, angleInRadians, Position.X, Position.Y);
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
        // If the bee hits the bottom of the stage, then wrap
        if (Position.Y > Constants.GAMEPLAY_Y)
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