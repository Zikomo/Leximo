using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using IETGames.Shorewood.Input;
using IETGames.Shorewood.Storage;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood
{
    class HighScoresDialog:MultiPageDialog
    {
        Texture2D textBackgroundTexture; 
        public HighScoresDialog()
            :base(new StringBuilder("High Scores"),
            Shorewood.Content.Load<Texture2D>(@"screens\menus\highScoresBackground"), 
            new Microsoft.Xna.Framework.Vector2(HighScores.scoreWidth,400))           
        {
            overlay = Shorewood.Content.Load<Texture2D>(@"screens\menus\highScoresOverlay");
            textBackgroundTexture = Shorewood.Content.Load<Texture2D>("screens\\textBackground");
        }


        public override void OnB(object sender, ButtonFireEventArgs args)
        {
            TransitionTo(parentDialogType,args.gameTime);            
        }

        public override void Activate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Shorewood.storage.HighScores.UpdateScoreTexture();
            renderedPages = Shorewood.storage.HighScores.renderedScores;
            base.Activate(gameTime);
        }
        public override void CustomDraw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Shorewood.IsTrial)
            {
                Vector2 stringBounds = Shorewood.fonts[FontTypes.MenuFont].MeasureString(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.HighScoresDisabledInTrial]) ;
                Vector2 position = center - stringBounds / 2;
                Vector2 txBounds = new Vector2(textBackgroundTexture.Width, textBackgroundTexture.Height);
                spriteBatch.Draw(textBackgroundTexture, new Rectangle((int)center.X, (int)center.Y, (int)stringBounds.X, (int)stringBounds.Y), null, Color.White, MathHelper.PiOver4, txBounds / 2, SpriteEffects.None, 0);
                spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuFont], 
                    Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.HighScoresDisabledInTrial],
                    center, Color.White, MathHelper.PiOver4, stringBounds / 2, 1, SpriteEffects.None, 0);
            }
            base.CustomDraw(spriteBatch, gameTime);
        }
    }
}