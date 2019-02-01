using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Invaders.Misc;

namespace Invaders.GameGraphics
{
    public class ParticleEngine
    {
        public ParticleEmitter[] Emitters { get => emitters.ToArray(); }

        private List<ParticleEmitter> emitters;

        public ParticleEngine()
        {
            emitters = new List<ParticleEmitter>();
        }

        public ParticleEmitter AddParticleSystem(Texture2D[] textures, Color color, Vector2 location, double emitterLifetime,
            int total,
            float minSize, float maxSize,
            float minVelocity, float maxVelocity,
            float rotation, float negativeRotationDeviation, float positiveRotationDeviation,
            float maxAngularVelocity,
            int minTTL, int maxTTL)
        {
            ParticleEmitter emitter = new ParticleEmitter(textures, color, location, emitterLifetime,
            total,
            minSize, maxSize,
            minVelocity, maxVelocity,
            rotation, negativeRotationDeviation, positiveRotationDeviation,
            maxAngularVelocity,
            minTTL, maxTTL);

            emitters.Add(emitter);
            return emitter;
        }

        public void RemoveParticleSystem(ParticleEmitter emitter)
        {
            emitters.Remove(emitter);
        }

        public void Update(double milliseconds)
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                ParticleEmitter e = emitters[i];
                if (!e.IsActive && e.Particles.Length == 0)
                {
                    emitters.Remove(e);
                    i--;
                }
            }

            foreach (var e in emitters)
            {
                e.Update(milliseconds);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var e in emitters)
            {
                if (e.IsActive || e.Particles.Length != 0)
                {
                    for (int index = 0; index < e.Particles.Length; index++)
                    {
                        e.Particles[index].Draw(spriteBatch);
                    }
                }
            }

        }
    }

    public class ParticleEmitter
    {
        public Particle[] Particles { get => particles.ToArray(); }
        public Vector2 Location { get; set; }
        public float Rotation { get; set; }
        public bool IsActive { get; set; }

        private Texture2D[] textures;
        private Color color;
        private List<Particle> particles;
        private Random random;
        private double startTime;
        private double emitterLifetime;

        private int totalAmount;
        private float minSize;
        private float maxSize;
        private float minVelocity;
        private float maxVelocity;
        private float negativeRotationDeviation;
        private float positiveRotationDeviation;
        private float maxAngularVelocity;
        private int minTTL; // TTL = "time to live" | for each particle
        private int maxTTL;

        public ParticleEmitter(Texture2D[] textures, Color color, Vector2 location, double emitterLifetime,
            int totalAmount,
            float minSize, float maxSize,
            float minVelocity, float maxVelocity,
            float rotation, float negativeRotationDeviation, float positiveRotationDeviation,
            float maxAngularVelocity,
            int minTTL, int maxTTL)
        {
            this.particles = new List<Particle>();
            this.random = new Random();
            this.IsActive = true;
            this.startTime = 0;
            this.emitterLifetime = emitterLifetime;

            this.textures = textures;
            this.color = color;
            this.Location = location;
            this.totalAmount = totalAmount;

            this.minSize = minSize;
            this.maxSize = maxSize;

            this.minVelocity = minVelocity;
            this.maxVelocity = maxVelocity;

            this.Rotation = rotation;
            this.negativeRotationDeviation = negativeRotationDeviation;
            this.positiveRotationDeviation = positiveRotationDeviation;

            this.maxAngularVelocity = maxAngularVelocity;
            this.minTTL = minTTL;
            this.maxTTL = maxTTL;
        }

        public void Update(double milliseconds)
        {
            if (startTime == 0)
            {
                startTime = milliseconds;
                if (emitterLifetime == -1.0F)
                {
                    emitterLifetime = float.MaxValue;
                }
            }

            emitterLifetime -= milliseconds - startTime;
            if (emitterLifetime <= 0)
            {
                IsActive = false;
            }
            else
            {
                for (int i = particles.Count; i < totalAmount; i++)
                {
                    if (IsActive)
                    {
                        particles.Add(GenerateNewParticle());
                    }
                }
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
                if (particles[i].TTL <= 0)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Length)];
            Vector2 position = Location;
            float angle = (float)random.NextDouble() * (positiveRotationDeviation - negativeRotationDeviation) + negativeRotationDeviation;
            angle += Rotation;

            Vector2 velocity = VectorMath.AngleToVector(angle);
            velocity *= (float)random.NextDouble() * (maxVelocity - minVelocity) + minVelocity;

            float angularVelocity = (float)random.NextDouble() * maxAngularVelocity;

            float size = (float)random.NextDouble() * (maxSize - minSize) + minSize;
            int ttl = Convert.ToInt32(random.NextDouble() * (maxTTL - minTTL)) + minTTL;

            return new Particle(texture, color, position, velocity, angle, angularVelocity, size, ttl);
        }
    }

    public class Particle
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public float Size { get; set; }
        public int TTL { get; set; }

        public Particle(Texture2D texture, Color color, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, float size, int ttl)
        {
            Texture = texture;
            Color = color;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destRectangle = new Rectangle(Position.ToPoint(), new Vector2(Size, Size).ToPoint());
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2F, Texture.Height / 2F);

            spriteBatch.Draw(Texture, destRectangle, sourceRectangle, Color,
                Angle, origin, SpriteEffects.None, 0);
        }
    }
}
