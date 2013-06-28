using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace IETGames.Shorewood.Utilities
{
    public static class DrawTimeSpanSpriteBatchExtension
    {
        private static readonly string seperator = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;

        public static Vector2 DrawTimeSpan(this SpriteBatch spriteBatch, SpriteFont spriteFont, TimeSpan value, Vector2 position, Color color)
        {
            if (spriteBatch == null) throw new ArgumentNullException("spriteBatch");
            if (spriteFont == null) throw new ArgumentNullException("spriteFont");

            Vector2 tempPosition = spriteBatch.DrawInt32(spriteFont, value.Minutes, position, color, 2);
            spriteBatch.DrawString(spriteFont, seperator, tempPosition, color);

            return spriteBatch.DrawInt32(spriteFont, value.Seconds, spriteFont.MeasureString(seperator) * Vector2.UnitX + tempPosition, color, 2);
        }
    }
}