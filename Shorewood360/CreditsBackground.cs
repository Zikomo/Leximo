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
using IETGames.Shorewood.Input;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CreditsBackground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        TimeSpan time;
        TimeSpan fadeDuration = new TimeSpan(0, 0, 2);
        Texture2D blackBars;
        Texture2D dimArea;
        public float alpha;
        float startFade = 0;
        float endFade = 1;
        bool isFading;
        EventHandler<ButtonFireEventArgs> onB;
        public CreditsBackground(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Visible = false;
            Enabled = false;
            onB = new EventHandler<ButtonFireEventArgs>(OnB);
        }

        public virtual void OnB(object nothing, ButtonFireEventArgs e)
        {
            if (!e.previouslyDown && ! isFading)
            {
                FadeOut(e.gameTime);
                if (Shorewood.freeSpellMode.Visible)
                {
                    Shorewood.freeSpellMode.Hide();
                }
            }
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            if (Visible)
            {
                Shorewood.inputHandler.AddEvent(Buttons.B, onB);
                Enabled = true;
                Shorewood.bloom.Enabled = true;
                Shorewood.bloom.Visible = true;
                startFade = 0;
                endFade = 1;
                isFading = true;
                time = TimeSpan.Zero;
                Shorewood.gameState = GameState.Credits;
            }
            else
            {

            }
            base.OnVisibleChanged(sender, args);
        }

        void FadeOut(GameTime gameTime)
        {
            startFade = 1;
            endFade = 0;
            isFading = true;
            time = TimeSpan.Zero;
            Shorewood.bloom.Enabled = false;
            Shorewood.bloom.Visible = false;
        }

        protected override void LoadContent()
        {
            blackBars = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
            dimArea = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
            Color[] blackBar = {Color.Black};
            Color[] dimPixels = {new Color(Color.Black,128)};
            blackBars.SetData<Color>(blackBar);
            dimArea.SetData<Color>(dimPixels);
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
            if (isFading)
            {
                time += gameTime.ElapsedGameTime;
                alpha = MathHelper.Lerp(startFade, endFade, (float)time.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds);
                alpha = MathHelper.Clamp(alpha, 0, 1);
                if (time.TotalMilliseconds > fadeDuration.TotalMilliseconds)
                {
                    isFading = false;
                    if (endFade == 0)
                    {
                        Visible = false;
                        Enabled = false;
                        Shorewood.creditsForeground.Enabled = false;
                        Shorewood.creditsForeground.Visible = false;
                        Shorewood.dialogManager.ShowDialog(DialogType.MainMenuDialog, gameTime);
                        Shorewood.gameState = GameState.Menu;
                        Shorewood.inputHandler.RemoveEvent(Buttons.B, onB);
                    }
                    else
                    {
                        Shorewood.creditsForeground.Enabled = true;
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 back = new Vector2(Shorewood.titleSafeArea.Left + 10, 700 - (int)(94f * alpha));
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(dimArea, new Rectangle(0, 0, 1280, 720), new Color(Color.White, (byte)(alpha * 255)));
            Shorewood.spriteBatch.Draw(blackBars, new Rectangle(0, 0, 1280, (int)(94f * alpha)), new Color(Color.White, (byte)(alpha * 255)));
            Shorewood.spriteBatch.Draw(blackBars, new Rectangle(0, 720 - (int)(94f * alpha), 1280, 94), new Color(Color.White, (byte)(alpha * 255)));
            Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont], Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogBack], back, new Color(Color.White, (byte)(alpha * 255)));
            Shorewood.spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}