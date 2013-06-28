using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Particle3DSample;

namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class UniversalParticleSystem : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix worldMatrix = Matrix.Identity;

        public ParticleSystem explosionParticles;
        public ParticleSystem explosionSmokeParticles;
        public ParticleSystem projectileTrailParticles;
        List<Projectile> projectiles = new List<Projectile>();
        List<FloatingPoints> points = new List<FloatingPoints>();
        public float pointDuration = 2000;
        
        public UniversalParticleSystem(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        protected override void LoadContent()
        {            
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 5, -5000, 5000);
            explosionParticles = new ExplosionParticleSystem(this.Game, Shorewood.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(this.Game, Shorewood.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(this.Game, Shorewood.Content);
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            explosionSmokeParticles.DrawOrder = 100;
            explosionParticles.Initialize();
            projectileTrailParticles.Initialize();
            explosionSmokeParticles.Initialize();
            projectileTrailParticles.SelfLoad();
            explosionParticles.SelfLoad();
            explosionSmokeParticles.SelfLoad();
            explosionParticles.SetCamera(viewMatrix, projectionMatrix);
            projectileTrailParticles.SetCamera(viewMatrix, projectionMatrix);
            explosionSmokeParticles.SetCamera(viewMatrix, projectionMatrix);
            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public Vector3 Unproject(Vector2 location)
        {
            return GraphicsDevice.Viewport.Unproject(new Vector3(location, 0), projectionMatrix, viewMatrix, worldMatrix);
        }

        public Vector2 Project(Vector3 location)
        {
            Vector3 twoDimensional = GraphicsDevice.Viewport.Project(location, projectionMatrix, viewMatrix, worldMatrix);
            return new Vector2(twoDimensional.X, twoDimensional.Y);
        }

        public void TriggerProjectile(FloatingPoints point)
        {
            projectiles.Add(new Projectile(explosionParticles,
                               explosionSmokeParticles,
                               projectileTrailParticles,
                               Unproject(point.startPosition)));
            //point.startTime = time;
            point.duration = new TimeSpan(0,0,0,0,(int)pointDuration);
            points.Add(point);            

        }

        public void TriggerExplosion(GameTime gameTime, Vector2 explosionLocation)
        {
            for (int i = 0; i < 10; i++)
                explosionParticles.AddParticle(Unproject(explosionLocation), new Vector3(4, 31, 0));

            for (int i = 0; i < 25; i++)
                explosionSmokeParticles.AddParticle(Unproject(explosionLocation), new Vector3(4, 31, 0));
        }

        private void UpdatePoints(GameTime gameTime)
        {
            for (int i = 0; i < points.Count; i++)
            {
                FloatingPoints point = points[i];
                point.Update(gameTime);
                point.position = Project(projectiles[i].position);
                point.origin = (Shorewood.fonts[FontTypes.FloatingPointsFont].MeasureString(points[i].text) / 2);
                points[i] = point;

                //Shorewood.pointsParticles.Trigger((point.position + pointBounds));
                //if (!point.isAlive)
                //{
                //    points.Remove(point);
                //}
            }
        }

        private void UpdateParticles(GameTime gameTime)
        {
            int i = 0;
            explosionParticles.ChangeTexture(Shorewood.Is1337Compliant);
            explosionSmokeParticles.ChangeTexture(Shorewood.Is1337Compliant);
            while (i < projectiles.Count)
            {
                Projectile projectile = projectiles[i];
                if (!projectile.Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    projectiles.RemoveAt(i);
                    points.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    projectiles[i] = projectile;
                    i++;
                }
            }
            explosionParticles.Update(gameTime);
            projectileTrailParticles.Update(gameTime);
            explosionSmokeParticles.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            UpdateParticles(gameTime);
            UpdatePoints(gameTime);
            base.Update(gameTime);
        }

        private void DrawPoints()
        {
            int i = 0;
            foreach (var point in points)
            {
                Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.FloatingPointsFont], point.text, point.position, Color.White, 0, point.origin, point.scale*Shorewood.foundWordAnimator.scale, SpriteEffects.None, 0);
                i++;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            projectileTrailParticles.Draw(gameTime);
            explosionParticles.Draw(gameTime);
            explosionSmokeParticles.Draw(gameTime);
            Shorewood.spriteBatch.Begin();
            DrawPoints();
            Shorewood.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}