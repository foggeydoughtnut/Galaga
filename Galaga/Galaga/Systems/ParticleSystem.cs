using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Galaga.Systems;

public class ParticleSystem : System
{
    private List<ParticleEmitter> _particleEmitters;
    private readonly Texture2D _particleTexture;

    public ParticleSystem(Texture2D particleTexture)
    {
        _particleEmitters = new();
        _particleTexture = particleTexture;
    }

    public override void Update(GameTime gameTime)
    {
        // Update all particle emitters and remove the fully completed ones
        List<ParticleEmitter> tempEmitters = new();
        for (int i = 0; i < _particleEmitters.Count; i++)
        {
            _particleEmitters[i].Update(gameTime);
            if (!_particleEmitters[i].isRemoveable())
            {
                tempEmitters.Add(_particleEmitters[i]);
            }
        }
        _particleEmitters = tempEmitters;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _particleEmitters.Count; i++)
        {
            _particleEmitters[i].Render(spriteBatch, 0, 0.4f);
        }
    }
    
    public void PlayerDeath(Point position)
    {
        ParticleEmitter particleEmitter = new(
            spawnPosition: new(position.X + _particleTexture.Width/2, position.Y + _particleTexture.Height/2),
            spawnPositionOffsetMax: Vector2.Zero,
            direction: 0,
            directionOffsetMax: 360f,
            minSpeed: 75f,
            maxSpeed: 100f,
            minScale: 0.25f,
            maxScale: 0.75f,
            minParticleLifetime: 0.1f,
            maxParticleLifetime: 0.25f,
            systemLifetime: TimeSpan.FromSeconds(0.1),
            spawnRate: TimeSpan.FromMilliseconds(1),
            particleTexture: _particleTexture
        );
        _particleEmitters.Add(particleEmitter);
    }
    
    public void EnemyDeath(Point position)
    {
        ParticleEmitter particleEmitter = new(
            spawnPosition: new(position.X + _particleTexture.Width/2, position.Y + _particleTexture.Height/2),
            spawnPositionOffsetMax: Vector2.Zero,
            direction: 0,
            directionOffsetMax: 360f,
            minSpeed: 75f,
            maxSpeed: 100f,
            minScale: 0.25f,
            maxScale: 0.75f,
            minParticleLifetime: 0.1f,
            maxParticleLifetime: 0.25f,
            systemLifetime: TimeSpan.FromSeconds(0.1),
            spawnRate: TimeSpan.FromMilliseconds(1),
            particleTexture: _particleTexture
        );
        _particleEmitters.Add(particleEmitter);
    }
}
