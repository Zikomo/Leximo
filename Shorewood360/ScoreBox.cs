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
using System.Text;
using System.Collections;

using IETGames.Shorewood.Utilities;
using IETGames.Shorewood.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics;

namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>    
    
    public class ScoreBox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public float countRate = 30;
        public int tallyStep = 1000;
        public Vector2 position;
        private int currentScore = 0;
        private int currentTally = 0;        
        private float nextUpdate = 0;
        private Vector2 scoreOrigin;
        private StringBuilder scoreBuilder;
        private Random random;
        private Texture2D scoreBoxBackground;
        private Vector2 scoreBoxOrigin;
        private SpriteFont font;
        private Vector2 scorePosition;
        private float scale = 1;
        protected RenderTarget2D scoreBoxTarget;
        protected Texture2D scoreBoxTexture;
        protected Body scoreBoxBody;
        protected Vector2 scoreBoxLowestPosition;
        protected Vector2 scoreBoxStartPosition;
        protected Matrix translationMatrix;
        Chain scoreBoxChain;
        bool changed = true;

        public ScoreBox(Game game)
            : base(game)
        {   
            scoreBuilder = new StringBuilder(60);            
            Enabled = false;
            Visible = false;
            random = new Random((int)DateTime.Now.Ticks);
        }
        int chainMass = 100000;
        protected int links = 15;
        protected void SetupPhysics()
        {
            scoreBoxStartPosition = new Vector2(-150, -125);
            scoreBoxLowestPosition = new Vector2(Shorewood.titleSafeArea.X, Shorewood.titleSafeArea.Y + 75);
            scoreBoxBody = BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, scoreBoxBackground.Width, scoreBoxBackground.Height, 10000);
            scoreBoxChain = new Chain(Shorewood.physicsEngine, (int)(links), scoreBoxStartPosition, 10, chainMass);
            scoreBoxChain.AttachEnd(new BodyNode(scoreBoxBody, scoreBoxBackground, false), 0);
            scoreBoxChain.AttachEnd(new BodyNode(BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, 10, 10, scoreBoxChain.chainMass), null), 0);
            for (int i = 1; i < links; i++)
            {
                scoreBoxChain.AttachEnd(new BodyNode(BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, 10, 10, scoreBoxChain.chainMass), null), 0);
            }
            scoreBoxChain.links[scoreBoxChain.links.Count - 1].body.IsStatic = true;
        }

        protected override void LoadContent()
        {
            position = new Vector2(Shorewood.titleSafeArea.X, Shorewood.titleSafeArea.Y);
            scoreBoxBackground = Shorewood.Content.Load<Texture2D>("screens\\scorebox");
            scoreBoxOrigin = new Vector2(scoreBoxBackground.Width / 2, scoreBoxBackground.Height / 2);
            font = Shorewood.fonts[FontTypes.ScoreFont];
            scorePosition = new Vector2(scoreBoxBackground.Width - 30, scoreBoxBackground.Height / 2f + 2.5f);
            scoreBoxTarget = new RenderTarget2D(GraphicsDevice, scoreBoxBackground.Width, scoreBoxBackground.Height, 1, scoreBoxBackground.Format);
            SetupPhysics();
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                scoreBoxChain.Reset();
            }
            base.OnEnabledChanged(sender, args);
        }

        public void Reset()
        {
            scoreBoxChain.Reset();
            changed = true;
            CurrentScore = 0;
            currentTally = 0;
        }

        public int CurrentScore
        {
            get
            {
                return currentScore;
            }
            set
            {
                currentScore = value;
            }
        }

        public void UpdateScore(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            
            if ((time > nextUpdate) && (currentTally < currentScore))
            {
                currentTally += (tallyStep / 2 + random.Next(tallyStep/2));
                currentTally = (int)MathHelper.Clamp(currentTally, 0, currentScore);
                nextUpdate = time + 1000 / countRate;
                scale = 1.1f;
                scoreBoxBody.ApplyImpulse(new Vector2(10, -chainMass * 50));
                changed = true;
                
            }
            else if (currentTally >= currentScore)
            {
                scale = 1;
            }
            scoreBuilder.Remove(0, scoreBuilder.Length);            
            scoreBuilder.Append(currentTally);
            Vector2 scoreBounds = font.MeasureString(scoreBuilder);
            scoreOrigin = new Vector2(scoreBounds.X, scoreBounds.Y / 2);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //scoreBoxBody.Rotation = MathHelper.Clamp(scoreBoxBody.Rotation, MathHelper.ToRadians(-20), MathHelper.ToRadians(20)); 
            scoreBoxBody.Position = Vector2.Clamp(scoreBoxBody.Position, scoreBoxStartPosition, scoreBoxLowestPosition);
            UpdateScore(gameTime);
            translationMatrix = Shorewood.UpdateTranslationMatrix(scoreBoxBody.Position + Vector2.UnitX * Shorewood.titleSafeArea.X);

            base.Update(gameTime);
        }



        //public void DrawToTexture(GameTime gameTime)
        //{
        //    RenderTarget2D temp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
        //    GraphicsDevice.SetRenderTarget(0, scoreBoxTarget);
        //    GraphicsDevice.Clear(Color.TransparentBlack);
        //    Shorewood.spriteBatch.Begin(); 
        //    DrawBackground(gameTime);
        //    DrawScore(gameTime);
        //    Shorewood.spriteBatch.End();
        //    GraphicsDevice.SetRenderTarget(0, temp);
        //    scoreBoxTexture = scoreBoxTarget.GetTexture();
        //    changed = false;
        //}

        public void DrawScore(GameTime gameTime)
        {
            Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.ScoreFont], scoreBuilder, scorePosition, Color.White,
                0, scoreOrigin, scale, SpriteEffects.None, 1);
            //Shorewood.spriteBatch.DrawInt32(Shorewood.fonts[FontTypes.ScoreFont], currentTally, Vector2.Zero, Color.White, 0);
        }

        public void DrawBackground(GameTime gameTime)
        {
            Shorewood.spriteBatch.Draw(scoreBoxBackground, Vector2.Zero, Color.White);
        }

        public void DrawTextureToScreen(GameTime gameTime)
        {//scoreBoxBody.Position + Vector2.UnitX * Shorewood.titleSafeArea.X
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(scoreBoxTexture, scoreBoxBody.Position + Vector2.UnitX * Shorewood.titleSafeArea.X, 
                null, Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            Shorewood.spriteBatch.End();
        }

        public override void Draw(GameTime gameTime)
        {
            //if (changed)
            //{
            //    DrawToTexture(gameTime);
            //}
            //DrawTextureToScreen(gameTime);           

            Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, translationMatrix);
            DrawBackground(gameTime);
            DrawScore(gameTime);
            Shorewood.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}