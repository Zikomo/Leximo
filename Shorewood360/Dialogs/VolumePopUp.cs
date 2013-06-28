using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood
{
    public class VolumePopUp:PopUpDialog
    {
        protected Texture2D volumeBackground;
        protected Texture2D volumeForeground;
        float volume=0;
        Vector2 bounds = new Vector2(180,20);
        public VolumePopUp(StringBuilder popUpTitle)
            : base(popUpTitle)
        {
            volumeBackground = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
            volumeForeground = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
            Color[] backgroundColor = { Color.Wheat };
            Color[] foregroundColor = { Color.CornflowerBlue };
            volumeBackground.SetData<Color>(backgroundColor);
            volumeForeground.SetData<Color>(foregroundColor);
        }

        public override void CustomDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Draw(volumeBackground, center, new Rectangle((int)(center.X - bounds.X), (int)(center.Y - bounds.Y), (int)bounds.X, (int)bounds.Y), Color.White);
            spriteBatch.Draw(volumeBackground, center, new Rectangle((int)(center.X - bounds.X), (int)(center.Y - bounds.Y), (int)(bounds.X*volume), (int)bounds.Y), Color.White);
            base.CustomDraw(spriteBatch, gameTime);
        }       
    }
}