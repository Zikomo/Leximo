#region File Description
//-----------------------------------------------------------------------------
// Projectile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using IETGames.Shorewood;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Particle3DSample
{
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    public struct Projectile
    {
        #region Constants

        const float trailParticlesPerSecond = 100;
        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 200;
        const float projectileMaxLifespan = 1.5f;
        const float projectileMinLifespan = 0.7f;
        float projectileLifespan;
        const float sidewaysVelocityRange = 60;
        const float verticalVelocityRange = 20;
        const float gravity = 15;

        #endregion

        #region Fields

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleEmitter trailEmitter;
        //TimeSpan sparkleTimeout;
        public Vector3 position;
        Vector3 velocity;
        float age;

        static Random random = new Random();

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles,
                          ParticleSystem projectileTrailParticles,
                          Vector3 startPosition)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;
            
            // Start at the origin, firing in a random (but roughly upward) direction.
            position = startPosition;//new Vector3((0-startPosition.X/2)/4, ((-startPosition.Y)/2)/4, 0);//Vector3.Zero;
            velocity = new Vector3();
            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * verticalVelocityRange+40;
            velocity.Z = 0;// (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            projectileLifespan = (float)random.NextDouble() * (projectileMaxLifespan - projectileMinLifespan) + projectileMinLifespan;
            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                               trailParticlesPerSecond, position);
            age = 0;
            
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * gravity;
            age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(gameTime, position);
            //sparkleTimeout += gameTime.ElapsedGameTime;
            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (age > projectileLifespan)
            {
                for (int i = 0; i < numExplosionParticles; i++)
                    explosionParticles.AddParticle(position, velocity);

                for (int i = 0; i < numExplosionSmokeParticles; i++)
                    explosionSmokeParticles.AddParticle(position, velocity);
                Shorewood.PlayBoom(position);
                Shorewood.PlaySparkles();
                //if (sparkleTimeout > Shorewood.sparkles.Duration)
                //{
                //    Shorewood.sparkles.Play();
                //    sparkleTimeout = TimeSpan.Zero;
                //}


                return false;
            }
                
            return true;
        }
    }
}
