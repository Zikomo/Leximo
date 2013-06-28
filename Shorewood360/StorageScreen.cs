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
    public class StorageScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D spinner;
        Texture2D screen;
        float rotation = 0;
        TimeSpan time = TimeSpan.Zero;
        TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
        public StorageScreen(Game game)
            : base(game)
        {
            IsLoading = false;
            Enabled = false;
            Visible = false;
            // TODO: Construct any child components here
        }

        public bool IsLoading
        {
            get;
            set;
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
            spinner = Shorewood.Content.Load<Texture2D>(@"Sprites\spinner");
            screen = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
            Color[] screenColor = { new Color(Color.Black, 128) };
            screen.SetData<Color>(screenColor);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here            
            time+= gameTime.ElapsedGameTime;
            if (time > duration)
            {
                time = TimeSpan.Zero;
            }
            rotation = MathHelper.Lerp(0, MathHelper.TwoPi, (float)time.TotalMilliseconds / (float)duration.TotalMilliseconds);



            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(screen, Shorewood.viewPortRectangle, Color.White);
            if (IsLoading)
            {
            }
            else
            {
            }
            Shorewood.spriteBatch.Draw(spinner,
                new Vector2(Shorewood.viewPortRectangle.Center.X, Shorewood.viewPortRectangle.Center.Y),
                null, Color.White, rotation,
                new Vector2(spinner.Width / 2, spinner.Height / 2), 1, SpriteEffects.None, 1);
            Shorewood.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}