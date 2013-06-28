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

using IETGames.Shorewood.Utilities;

namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TimerRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D timerBackground;
        Texture2D timerTexture;
        RenderTarget2D timerTarget;
        TimeSpan timeOfLastUpdate = TimeSpan.Zero;
        Vector2 nextChallengeTimePosition;
        Vector2 nextChallengeTextPosition;
        public TimeSpan NextChallenge
        {
            get;
            set;
        }

        public bool TimerChanged
        {
            get;
            set;
        }

        public void OnTimerChange(GameTime time, object nothing)
        {
            TimerChanged = true;
            NextChallenge -= TimeSpan.FromSeconds(1);
            if (NextChallenge < TimeSpan.Zero)
            {
                NextChallenge = TimeSpan.Zero;
            }
            if (!Visible || !Enabled)
            {
                Visible = true;
                Enabled = true;
            }
            
        }


        public TimerRenderer(Game game)
            : base(game)
        {
            TimerChanged = true;
            Enabled = false;
            Visible = false;
            NextChallenge = TimeSpan.Zero;
            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            timerBackground = Shorewood.Content.Load<Texture2D>("screens\\timerbox");
            timerTarget = new RenderTarget2D(GraphicsDevice, timerBackground.Width, timerBackground.Height, 1, timerBackground.Format);
            nextChallengeTextPosition = new Vector2(40, 60);
            nextChallengeTimePosition = Shorewood.fonts[FontTypes.ScoreFont].MeasureString(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.NextChallenge])*Vector2.UnitX  + nextChallengeTextPosition;
            
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }

        void DrawBackground(GameTime gameTime)
        {
            Shorewood.spriteBatch.Draw(timerBackground, Vector2.Zero, Color.White);

        }

        void DrawTimer(GameTime gameTime)
        {            
            Shorewood.spriteBatch.DrawTimeSpan(Shorewood.fonts[FontTypes.MenuFont], Shorewood.gameplayTimer.CountdownTime, new Vector2(40, 10), Color.White);
        }

        void DrawToTexture(GameTime gameTime)
        {
            if (TimerChanged)
            {
                RenderTarget2D temp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
                GraphicsDevice.SetRenderTarget(0, timerTarget);
                GraphicsDevice.Clear(Color.TransparentBlack);
                Shorewood.spriteBatch.Begin();

                DrawBackground(gameTime);
                DrawNextLetter();
                DrawNextChallenge();
                DrawTimer(gameTime);
                TimerChanged = false;
                
                Shorewood.spriteBatch.End();
                GraphicsDevice.SetRenderTarget(0, temp);
                timerTexture = timerTarget.GetTexture();
            }
        }

        private void DrawNextChallenge()
        {
            if (Shorewood.Difficulty != Difficulty.FreeSpell)
            {
                Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.ScoreFont],
                    Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.NextChallenge],
                    nextChallengeTextPosition, Color.White);

                Shorewood.spriteBatch.DrawInt32(Shorewood.fonts[FontTypes.ScoreFont], NextChallenge.Seconds, nextChallengeTimePosition, Color.White, 2);
            }
        }

        private void DrawNextLetter()
        {
            if (Shorewood.normalGameplayRenderer.nextLetter != Letter.EmptyLetter && Shorewood.normalGameplayRenderer.nextLetter.texture != null)
            {
                Shorewood.spriteBatch.Draw(Shorewood.normalGameplayRenderer.nextLetter.texture, new Vector2(200, 8), null, Color.White, 0, Vector2.Zero, 0.39f, SpriteEffects.None, 0);
            }
        }

        void DrawToScreen(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(timerTexture, new Vector2(875, 75), Color.White);
            Shorewood.spriteBatch.End();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawToTexture(gameTime);
            DrawToScreen(gameTime);            
            base.Draw(gameTime);
        }
    }
}