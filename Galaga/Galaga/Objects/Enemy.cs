using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Objects;

public abstract class Enemy : Object
{
    public List<Vector2> EntrancePath;
    public Vector2? Destination;
    public double VelocityVector = 150;
    public bool ReachedEndOfEntrancePath;

    private float attackTimer = 0f;
    private float attackDelay; // Delay until enemy attacks once it reaches the start position
    private bool startAttackTimer = false;

    private float shootTimer = 0f;
    private float shootDelay = 0.25f; // Delay until enemy shoots
    private bool startShootTimer = false;
    private bool doneShooting = false;
    private int numberOfShotsFired = 0;

    public int health { get; set; } = 1;

    public bool attack = false;

    public PlayerShip Player;
    private readonly Random rnd;

    private BulletSystem _bulletSystem;

    private bool _canAttack;

    private double _elapsedTimeTotal;
    private Vector2 _breathingVelocity;
    private Vector2 _breathingMovement;
    public Point StartAttackPos;

    private AudioSystem _audioSystem;



    public Enemy(Point position, Point dimensions, Texture2D texture, int numAnimations, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack, AudioSystem audioSystem) : base(position, dimensions, texture, animationTimeMilliseconds, debugTexture, numAnimations)
    {
        EntrancePath = new List<Vector2>();
        ReachedEndOfEntrancePath = false;
        Player = player;
        rnd = new();
        _bulletSystem = bulletSystem;
        _canAttack = canAttack;
        _elapsedTimeTotal = 0;
        attackDelay = (float)(rnd.NextDouble() * 10f) + 5f; // Generates a random float between 5 and 15 to be used as the delay for attacking
        _breathingMovement = new();
        StartAttackPos = new();
        _audioSystem = audioSystem;
    }

    // This one is for boss Galaga since it has more than 1 texture
    public Enemy(Point position, Point dimensions, List<Texture2D> textures, int numAnimations, int animationTimeMilliseconds, Texture2D debugTexture, PlayerShip player, BulletSystem bulletSystem, bool canAttack, AudioSystem audioSystem) : base(position, dimensions, textures[0], animationTimeMilliseconds, debugTexture, numAnimations)
    {
        EntrancePath = new List<Vector2>();
        ReachedEndOfEntrancePath = false;
        Player = player;
        rnd = new();
        _bulletSystem = bulletSystem;
        _canAttack = canAttack;
        _breathingMovement = new();
        StartAttackPos = new();
        attackDelay = (float)(rnd.NextDouble() * 10f) + 5f; // Generates a random float between 5 and 15 to be used as the delay for attacking
        _elapsedTimeTotal = 0;
        _audioSystem = audioSystem;

    }

    public override void Update(TimeSpan elapsedTime)
    {
        base.Update(elapsedTime);
        _elapsedTimeTotal += elapsedTime.TotalSeconds;
        if (ReachedEndOfEntrancePath && !EnemySystem.Breathing)
        {
            _breathingVelocity = new(EnemySystem.HorizontalDirection, 0);
            _breathingVelocity *= 90;
            _breathingVelocity *= (float)elapsedTime.TotalSeconds;
            _breathingMovement += _breathingVelocity;

            if (!attack)
            {
                if (Math.Abs(_breathingMovement.X) > 4)
                {
                    Position.X += _breathingMovement.X < 0 ? 1 : -1;
                    StartAttackPos.X += _breathingMovement.X < 0 ? 1 : -1;
                    _breathingMovement.X = 0;
                }
            }
            else
            {
                if (Math.Abs(_breathingMovement.X) > 4)
                {
                    StartAttackPos.X += _breathingMovement.X < 0 ? 1 : -1;
                    _breathingMovement.X = 0;
                }
            }
        }
        if (EnemySystem.Breathing)
        {
            if (!attack)
            {
                if (Position.X < Constants.GAMEPLAY_X / 2)
                {
                    _breathingVelocity = new(1f * EnemySystem.BreathingDirection, -1.0f * EnemySystem.BreathingDirection);
                    _breathingVelocity *= 90;
                    _breathingVelocity *= (float)elapsedTime.TotalSeconds;
                } 
                else
                {
                    _breathingVelocity = new(-1.0f * EnemySystem.BreathingDirection, -1.0f * EnemySystem.BreathingDirection);
                    _breathingVelocity *= 90;
                    _breathingVelocity *= (float)elapsedTime.TotalSeconds;
                }
                _breathingMovement += _breathingVelocity;
                if (Math.Abs(_breathingMovement.Y) > 4 && Math.Abs(_breathingMovement.X) > 2)
                {
                    Position.X += _breathingMovement.X < 0 ? 1 : -1;
                    Position.Y += _breathingMovement.Y < 0 ? 1 : -1;
                    StartAttackPos.X += _breathingMovement.X < 0 ? 1 : -1;
                    StartAttackPos.Y += _breathingMovement.Y < 0 ? 1 : -1;
                    _breathingMovement = Vector2.Zero;
                }
            }
            else
            {
                if (Position.X < Constants.GAMEPLAY_X / 2)
                {
                    _breathingVelocity = new(1f * EnemySystem.BreathingDirection, -1.0f * EnemySystem.BreathingDirection);
                    _breathingVelocity *= 90;
                    _breathingVelocity *= (float)elapsedTime.TotalSeconds;
                }
                else
                {
                    _breathingVelocity = new(-1.0f * EnemySystem.BreathingDirection, -1.0f * EnemySystem.BreathingDirection);
                    _breathingVelocity *= 90;
                    _breathingVelocity *= (float)elapsedTime.TotalSeconds;
                }
                _breathingMovement += _breathingVelocity;
                if (Math.Abs(_breathingMovement.Y) > 4 && Math.Abs(_breathingMovement.X) > 2)
                {
                    StartAttackPos.X += _breathingMovement.X < 0 ? 1 : -1;
                    StartAttackPos.Y += _breathingMovement.Y < 0 ? 1 : -1;
                    _breathingMovement = Vector2.Zero;
                }
            }
        }

        if (_canAttack) // Bonus round enemies can't attack
        {
            if (attack)
            {
                CalculateAttackPath();

                // Start timer for when it will shoot
                startShootTimer = true;
                if (startShootTimer && !doneShooting)
                {
                    shootTimer += (float)elapsedTime.TotalSeconds;
                    if (shootTimer >= shootDelay)
                    {
                        Vector2 enemyPos = new(Position.X, Position.Y + Constants.CHARACTER_DIMENSIONS);
                        Vector2 playerPos = new(Player.Position.X, Player.Position.Y);
                        Vector2 bulletVelocity = playerPos - enemyPos;
                        bulletVelocity.Normalize();
                        _bulletSystem.FireEnemyBullet(new Point(Position.X, Position.Y+Constants.CHARACTER_DIMENSIONS), bulletVelocity);
                        numberOfShotsFired++;
                        shootTimer -= 0.2f;
                    }
                    if (numberOfShotsFired > 1)
                    {
                        doneShooting = true;
                        startShootTimer = false;
                    }
                }

                return;
            }
        }

        if (!EntrancePath.Any() && Destination == null)
        {
            ReachedEndOfEntrancePath = true;
            startAttackTimer = true;
            if (startAttackTimer && !attack)
            {
                attackTimer += (float)elapsedTime.TotalSeconds;

                if (attackTimer >= attackDelay)
                {
                    Attack();
                    startAttackTimer = false;
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
        startAttackTimer = true;
        attackTimer = 0f;

        doneShooting = false;
        startShootTimer = true;
        shootTimer= 0f;
        numberOfShotsFired = 0;
    }

    protected virtual void Attack()
    {
        attack = true;
        StartAttackPos = Position;
        if (_canAttack)
            _audioSystem.PlaySoundEffect("enemyFly");
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