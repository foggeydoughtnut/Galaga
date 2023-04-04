using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Utilities
{
    public class ParticleEmitter
    {
        struct Particle
        {
            public Vector2 position;
            public Vector2 velocity;
            public TimeSpan lifetime;
            public float scale;
        }

        public int name;

        // Position
        public Vector2 spawnPosition;
        public Vector2 spawnPositionOffsetMax;

        // Direction
        public float direction;
        public float directionOffsetMax;

        // Speeds
        public float minSpeed;
        public float maxSpeed;

        // Scales
        public float minScale;
        public float maxScale;

        // Lifetime
        public float minParticleLifetime;
        public float maxParticleLifetime;
        public TimeSpan systemLifetime; // stops spawning them at this point, does not disappear at this time
        private TimeSpan elapsedTime;
        public TimeSpan spawnRate; // # timespan between spawning particles

        // Texture
        public Texture2D particleTexture;

        private List<Particle> particles;

        private List<Particle> updateList;

        private readonly MyRandom myRandom;

        public ParticleEmitter(Vector2 spawnPosition, Vector2 spawnPositionOffsetMax, float direction, float directionOffsetMax, float minSpeed, float maxSpeed, float minScale, float maxScale, float minParticleLifetime, float maxParticleLifetime, TimeSpan systemLifetime, TimeSpan spawnRate, Texture2D particleTexture)
        {
            this.spawnPosition = spawnPosition;
            this.spawnPositionOffsetMax = spawnPositionOffsetMax;
            this.direction = direction;
            this.directionOffsetMax = directionOffsetMax;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.minParticleLifetime = minParticleLifetime;
            this.maxParticleLifetime = maxParticleLifetime;
            this.systemLifetime = systemLifetime;
            this.spawnRate = spawnRate;
            this.particleTexture = particleTexture;
            elapsedTime = TimeSpan.Zero;

            myRandom = new();
            particles = new();
            updateList = new();
        }

        private void SpawnParticle()
        {
            // Use this for covering an area
            //Vector2 offset = myRandom.NextVector();
            /*            offset.X = Math.Abs(offset.X * spawnPositionOffsetMax.X);
                        offset.Y = Math.Abs(offset.Y * spawnPositionOffsetMax.Y);*/

            Vector2 offset = myRandom.NextCircleVector();

            float direction = this.direction + myRandom.NextRange(-1, 1) * this.directionOffsetMax;
            float speed = myRandom.NextRange(minSpeed, maxSpeed);
            Vector2 finalDirection = new(MathF.Cos(direction) * speed, MathF.Sin(direction) * speed);

            Particle particle = new()
            {
                position = offset + spawnPosition,
                velocity = finalDirection,
                lifetime = TimeSpan.FromSeconds(myRandom.NextRange(minParticleLifetime, maxParticleLifetime)),
                scale = myRandom.NextRange(minScale, maxScale),
            };

            particles.Add(particle);
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            this.systemLifetime -= gameTime.ElapsedGameTime;

            // Add all the particles that should be added
            while (this.systemLifetime > TimeSpan.Zero && elapsedTime > this.spawnRate)
            {
                SpawnParticle();

                elapsedTime -= this.spawnRate;
            }

            // Update all the particles that need updating
            updateList = particles;
            particles = new();
            for (int i = 0; i < updateList.Count; i++)
            {
                Particle particle = updateList[i];
                particle.lifetime -= gameTime.ElapsedGameTime;
                if (particle.lifetime <= TimeSpan.Zero)
                {
                    continue;
                }
                particle.position += particle.velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                particles.Add(particle);
            }
            updateList = new();
        }

        public void Render(SpriteBatch m_spriteBatch, float rotation, float depth)
        {
            //Debug.WriteLine(particles.Count);
            foreach (Particle particle in particles)
            {
                m_spriteBatch.Draw(
                    this.particleTexture,
                    particle.position,
                    null,
                    Color.Red,
                    rotation,
                    new Vector2(this.particleTexture.Width / 2, this.particleTexture.Height / 2),
                    particle.scale,
                    SpriteEffects.None,
                    depth
                );
            }
            // render particle using
            /*                particle.position;
                            particle.scale;
                            this.particleTexture;*/
        }

        public bool isRemoveable()
        {
            return particles.Count == 0 && this.systemLifetime <= TimeSpan.Zero;
        }
    }
}
