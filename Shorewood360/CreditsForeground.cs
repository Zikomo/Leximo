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
    public class CreditsForeground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle clipping = new Rectangle(0, 94, 1280, 533);
        TimeSpan time = TimeSpan.Zero;
        TimeSpan zikomoStartTime = TimeSpan.Zero;
        TimeSpan creditsStart = TimeSpan.Zero;
        TimeSpan creditsDuration = new TimeSpan(0, 0, 25);
        Texture2D me;
        Texture2D credits;
        Texture2D logo;
        Texture2D social;
        Vector2 zikomoPosition = Vector2.Zero;
        Vector2 creditsStartPosition = Vector2.Zero;
        Vector2 creditsDestination = Vector2.Zero;
        Vector2 creditsPosition = Vector2.Zero;
        Vector2 zikomoDestination = Vector2.Zero;
        Vector2 zikomoOrigin = Vector2.Zero;
        float zikomoStep = 0;
        TimeSpan zikomoInDuration = new TimeSpan(0, 0, 0,0,700);
        TimeSpan pause = new TimeSpan(0, 0, 0, 0, 5200);
        float zikomoRotation = 0;
        bool loaded = false;
        public CreditsForeground(Game game)
            : base(game)
        {
            Enabled = false;
            Visible = false;
        }
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled && loaded)
            {
                Visible = true;
                time = TimeSpan.Zero;
                zikomoStartTime = new TimeSpan(0, 0, 0, 0, 500);
                zikomoDestination = new Vector2(Shorewood.viewPortRectangle.Center.X,
                    Shorewood.viewPortRectangle.Center.Y);
                zikomoRotation = 0;
                creditsPosition = creditsStartPosition = new Vector2(Shorewood.viewPortRectangle.Center.X - credits.Width / 2, 720);
                creditsDestination = new Vector2(creditsStartPosition.X, -credits.Height);
                zikomoStep = 0;
            }

            base.OnEnabledChanged(sender, args);
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

        protected override void LoadContent()
        {
            me = Shorewood.Content.Load<Texture2D>(@"Sprites\zikomo");
            credits = Shorewood.Content.Load<Texture2D>(@"Sprites\credits");
            logo = Shorewood.Content.Load<Texture2D>(@"screens\NinthPlanetSplashTwo");
            social = Shorewood.Content.Load<Texture2D>(@"screens\social");
            zikomoOrigin = new Vector2(me.Width / 2, me.Height / 2);
            loaded = true;
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime;            
            if (time > zikomoStartTime)
            {                
                if (time < (zikomoInDuration + zikomoStartTime))
                {
                    zikomoRotation = MathHelper.SmoothStep(0, MathHelper.TwoPi, (float)((time.TotalMilliseconds - zikomoStartTime.TotalMilliseconds) / zikomoInDuration.TotalMilliseconds));
                    zikomoStep = MathHelper.SmoothStep(0, 1, (float)((time.TotalMilliseconds - zikomoStartTime.TotalMilliseconds) / zikomoInDuration.TotalMilliseconds));
                    if (!Shorewood.freeSpellMode.Visible)
                    {
                        Shorewood.freeSpellMode.Show();
                    }
                }
                else if ((time > pause)&&(time < (pause + creditsDuration)))
                {
                    if (Shorewood.freeSpellMode.Visible)
                    {
                        Shorewood.freeSpellMode.Hide();
                    }
                    zikomoRotation = MathHelper.SmoothStep(0, MathHelper.TwoPi, (float)((time.TotalMilliseconds - pause.TotalMilliseconds) / zikomoInDuration.TotalMilliseconds));
                    zikomoStep = MathHelper.SmoothStep(1, 0, (float)((time.TotalMilliseconds - pause.TotalMilliseconds) / zikomoInDuration.TotalMilliseconds));
                    creditsPosition = Vector2.Lerp(creditsStartPosition, creditsDestination, (float)((time.TotalMilliseconds - pause.TotalMilliseconds) / creditsDuration.TotalMilliseconds));
                }

            }
            
            base.Update(gameTime);
        }

        void DrawZikomo()
        {
            Shorewood.spriteBatch.Draw(me, zikomoDestination, null, new Color(Color.White, (byte)(zikomoStep * 255 * Shorewood.creditsBackground.alpha)), zikomoRotation, zikomoOrigin,10 - zikomoStep*9, SpriteEffects.None, 0);
        }

        private void DrawCredits()
        {
            Shorewood.spriteBatch.Draw(credits, creditsPosition, new Color(Color.White, (byte)(255 * Shorewood.creditsBackground.alpha)));
            Shorewood.spriteBatch.Draw(logo, Vector2.UnitY *(creditsPosition.Y +  credits.Height), new Color(Color.White, (byte)(255 * Shorewood.creditsBackground.alpha)));
            Shorewood.spriteBatch.Draw(social, Vector2.UnitY * (creditsPosition.Y + credits.Height), new Color(Color.White, (byte)(255 * Shorewood.creditsBackground.alpha)));
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle oldScissorRectangle = Shorewood.graphics.GraphicsDevice.ScissorRectangle;
            bool oldScissorTest = Shorewood.graphics.GraphicsDevice.RenderState.ScissorTestEnable;
            Shorewood.graphics.GraphicsDevice.RenderState.ScissorTestEnable = true;
            Shorewood.graphics.GraphicsDevice.ScissorRectangle = clipping;
            Shorewood.spriteBatch.Begin();
            DrawZikomo();
            DrawCredits();
            Shorewood.spriteBatch.End();            
            Shorewood.graphics.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
            Shorewood.graphics.GraphicsDevice.RenderState.ScissorTestEnable = oldScissorTest;
            base.Draw(gameTime);
        }
    }
}