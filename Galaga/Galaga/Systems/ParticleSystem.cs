using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Galaga.Systems;

public class ParticleSystem : System
{
    List<ParticleEmitter> particleEmitters;
    Texture2D particleTexture;


    public ParticleSystem(Texture2D particleTexture)
    {
        this.particleEmitters = new();
        this.particleTexture = particleTexture;
    }

    public override void Update(GameTime gameTime)
    {
        // Update all particle emitters and remove the fully completed ones
        List<ParticleEmitter> tempEmitters = new();
        for (int i = 0; i < particleEmitters.Count; i++)
        {
            particleEmitters[i].Update(gameTime);
            if (!particleEmitters[i].isRemoveable())
            {
                tempEmitters.Add(particleEmitters[i]);
            }
        }
        particleEmitters = tempEmitters;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < particleEmitters.Count; i++)
        {
            particleEmitters[i].Render(spriteBatch, 0, 0.4f);
        }
    }
    
    public void PlayerDeath(Point position)
    {
        ParticleEmitter particleEmitter = new(
            spawnPosition: new(position.X + particleTexture.Width/2, position.Y + particleTexture.Height/2),
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
            particleTexture: particleTexture
        );
        this.particleEmitters.Add(particleEmitter);
    }
    
    public void EnemyDeath(Point position)
    {
        
    }
}
