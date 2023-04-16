using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public abstract class Enemy : Object
{
    public List<Vector2> EntrancePath;
    public Vector2? Destination;
    public double VelocityVector = 150;
    public bool ReachedEndOfEntrancePath;

    private float timer = 0f;
    private float delay; // Delay until enemy attacks once it reaches the start position
    private bool startTimer = false;

    private bool attack = false;

    public PlayerShip Player;
    private readonly Random rnd;

    public Enemy(Point position, Point dimensions, Texture2D texture, int numAnimations, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player) : base(position, dimensions, texture, animationTimeMilliseconds, debugTexture, numAnimations)
    {
        EntrancePath = new List<Vector2>();
        ReachedEndOfEntrancePath = false;
        Player = player;
        rnd = new();
        
        delay = (float)(rnd.NextDouble() * 10f) + 5f; // Generates a random float between 5 and 15 to be used as the delay for attacking
    }

    public override void Update(TimeSpan elapsedTime)
    {
        base.Update(elapsedTime);

        if (attack)
        {
            CalculateAttackPath();
            return;
        }

        if (!EntrancePath.Any() && Destination == null)
        {
            ReachedEndOfEntrancePath = true;
            startTimer = true;
            if (startTimer && !attack)
            {
                timer += (float)elapsedTime.TotalSeconds;

                if (timer >= delay)
                {
                    Attack();
                    startTimer = false;
                }

            }
            if (attack) return;
            ResetVelocity();
            return;
        }
        if (Destination is not null && !EntrancePath.Any())
            CalculateNewVelocityForDestination();
        else if(EntrancePath.Any())
            CalculateNewVelocityForEntrancePath();

    }

    public void ResetAttackTimer()
    {
        attack = false;
        startTimer = true;
        timer = 0f;
    }

    protected virtual void Attack()
    {
        attack = true;
    }

    public virtual void CalculateAttackPath()
    {
    }

    private void ResetVelocity()
    {
        VelocityX = 0;
        VelocityY = 0;
    }
    
    private void CalculateNewVelocityForEntrancePath()
    {
        Vector2 nextPoint = EntrancePath.First();
        float xDistance = nextPoint.X - Position.X;
        float xDistanceSquared = xDistance * xDistance;
        float yDistance = nextPoint.Y - Position.Y;
        float yDistanceSquared = yDistance * yDistance;
        if (xDistanceSquared + yDistanceSquared < 50)
            EntrancePath.RemoveAt(0);
        double totalDistance = Math.Sqrt(xDistanceSquared + yDistanceSquared);
        VelocityX = VelocityVector * xDistance / totalDistance;
        VelocityY = VelocityVector * yDistance / totalDistance;
    }

    private void CalculateNewVelocityForDestination()
    {
        if (Destination is null)
            return;
        Vector2 nextPoint = Destination.Value;
        float xDistance = nextPoint.X - Position.X;
        float xDistanceSquared = xDistance * xDistance;
        float yDistance = nextPoint.Y - Position.Y;
        float yDistanceSquared = yDistance * yDistance;
        double totalDistance = Math.Sqrt(xDistanceSquared + yDistanceSquared);
        if (xDistanceSquared + yDistanceSquared < 50)
            Destination = null;
        VelocityX = VelocityVector * xDistance / totalDistance;
        VelocityY = VelocityVector * yDistance / totalDistance;
    }
}