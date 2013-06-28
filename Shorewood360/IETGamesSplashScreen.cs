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


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IETGamesSplashScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D logo;

        Texture2D letterI;
        Vector2 letterIPosition;
        Vector2 letterIEndPosition;
        Vector2 letterIStartPosition;

        Texture2D letterE;
        Vector2 letterEPosition;
        Vector2 letterEEndPosition;
        Vector2 letterEStartPosition;

        Texture2D letterT;
        Vector2 letterTPosition;
        Vector2 letterTEndPosition;
        Vector2 letterTStartPosition;

        SpriteBatch spriteBatch;

        float startTime;

        private float letterAnimationDuration = 0;
        private float fadeInStart = 0;
        private float fadeInDuration = 0;
        public float duration = 3500;
        public float scale = 1;
        public Vector2 position = Vector2.Zero;
        public Vector2 realPadding = Vector2.Zero;
        public bool animationFinished = false;
        public bool animationStarted = false;
        private Color fade = Color.White;
        public IETGamesSplashScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public void Show()
        {
            this.Visible = true;
            this.Enabled = true;
        }
        public void Hide()
        {
            this.Visible = false;
            this.Enabled = false;
        }

        protected override void LoadContent()
        {
            logo = Shorewood.Content.Load<Texture2D>("screens\\ietgameslogo");
            letterI = Shorewood.Content.Load<Texture2D>("screens\\logoI");
            letterE = Shorewood.Content.Load<Texture2D>("screens\\logoE");
            letterT = Shorewood.Content.Load<Texture2D>("screens\\logoT");
            
            realPadding.Y = PlatformDisplaySettings.height / 2 - (logo.Height / 2) * scale;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
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

        public void StartAnimation(float startTime)
        {
            letterIPosition = letterIStartPosition = position + new Vector2(-500, letterI.Height * 2 / 3);
            letterEPosition = letterEStartPosition = position + new Vector2(0, -500);
            letterTPosition = letterTStartPosition = position + new Vector2(1000, letterI.Height * 2 / 3);

            letterIEndPosition = position + (new Vector2(-letterI.Width / 2, letterI.Height * 2 / 3)) + Vector2.UnitX * 50;
            letterEEndPosition = position + Vector2.UnitX * 50;
            letterTEndPosition = position + (new Vector2(letterI.Width / 2, letterI.Height * 2 / 3)) + Vector2.UnitX * 50;

            fade.A = 0;
            fadeInDuration = duration / 2;
            letterAnimationDuration = duration / 2;
            fadeInStart = 0;
            this.startTime = startTime;
            animationFinished = false;
            animationStarted = true;
        }

        private void UpdateI(float step)
        {
            letterIPosition = Vector2.SmoothStep(letterIPosition, letterIEndPosition, step);
        }
        private void UpdateE(float step)
        {
            letterEPosition = Vector2.SmoothStep(letterEPosition, letterEEndPosition, step);
            
        }
        private void UpdateT(float step)
        {
            letterTPosition = Vector2.SmoothStep(letterTPosition, letterTEndPosition, step);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!animationFinished)
            {
                float time = (float)gameTime.TotalGameTime.TotalMilliseconds - startTime;
                float stepLetterDuration = time / letterAnimationDuration;

                if (time < letterAnimationDuration)
                {
                    UpdateI(stepLetterDuration);
                    UpdateE(stepLetterDuration);
                    UpdateT(stepLetterDuration);
                }
                if (time > fadeInStart)
                {
                    float stepFade = (time) / duration;
                    fade.A = (byte)(stepFade * 255);
                }
                if (time > duration)
                {
                    animationFinished = true;
                    animationStarted = false;
                    Visible = false;
                    Enabled = false;
                }
            }
            base.Update(gameTime);
        }

        private void DrawI()
        {
            spriteBatch.Draw(letterI, letterIPosition * scale + realPadding, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        private void DrawE()
        {
            spriteBatch.Draw(letterE, letterEPosition * scale + realPadding, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        private void DrawT()
        {
            spriteBatch.Draw(letterT, letterTPosition * scale + realPadding, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        private void DrawLogo()
        {
            spriteBatch.Draw(logo, position * scale + realPadding, null, fade, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);
            DrawI();
            DrawE();
            DrawT();
            DrawLogo();
            spriteBatch.End();           

        }
    }
}