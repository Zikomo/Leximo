using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace IETGames.Shorewood
{
    public class ModalDialog:Dialog
    {        
        public SpriteFont footerFonts = null;
        public Color footerLeftColor = Color.White;
        public Color footerMiddleColor = Color.White;
        public Color footerRightColor = Color.White;

        protected StringBuilder footerLeft = null;
        protected StringBuilder footerMiddle = null;
        protected StringBuilder footerRight = null;
        
        protected Vector2 footerLeftSize;
        protected Vector2 footerMiddleSize;
        protected Vector2 footerRightSize;

        protected Vector2 footerLeftPosition;
        protected Vector2 footerMiddlePosition;
        protected Vector2 footerRightPosition;

        private Vector2 footerLeftOrigin;
        private Vector2 footerMiddleOrigin;
        private Vector2 footerRightOrigin;
        

        public ModalDialog(StringBuilder dialogTitle, string dialogBackgroundContentLocation, string dialogHighlightContentLocation)
            :base (dialogTitle, dialogBackgroundContentLocation, dialogHighlightContentLocation)
        {
            isModal = true;
            wrapText = true;
            staticTextColor = Color.Black;
            footerFonts = Shorewood.fonts[FontTypes.MenuButtonFont];
            footerLeftPosition = new Vector2(bounds.Left, bounds.Bottom - 30);
            footerLeftOrigin = new Vector2(0, 0);
            footerMiddlePosition = new Vector2(center.X, bounds.Bottom - 30);
            footerMiddleOrigin = new Vector2(25, 0);
            footerRightPosition = new Vector2(bounds.Right - 50, bounds.Bottom - 30);
            footerRightOrigin = new Vector2(50, 0);
            
        }

        public StringBuilder FooterLeft
        {
            get
            {
                return footerLeft;
            }
            set
            {
                footerLeft = value;
                footerLeftPosition.Y = bounds.Bottom - footerFonts.MeasureString(footerLeft).Y - 3;
            }
        }

        public StringBuilder FooterRight
        {
            get
            {
                return footerRight;
            }
            set
            {
                footerRight = value;
                Vector2 rightBounds = footerFonts.MeasureString(footerRight);
                footerRightPosition.Y = bounds.Bottom - rightBounds.Y - 3;
                footerRightPosition.X = bounds.Right - rightBounds.X;
                //footerRightOrigin.X = rightBounds.X;
            }
        }

        public StringBuilder FooterMiddle
        {
            get
            {
                return footerMiddle;
            }
            set
            {
                footerMiddle = value;
                Vector2 middleBounds = footerFonts.MeasureString(footerMiddle);
                footerMiddleOrigin = middleBounds / 2;
                footerMiddlePosition.X = center.X;
                footerMiddlePosition.Y = bounds.Bottom - middleBounds.Y + footerMiddleOrigin.Y;
            }
        }

        public override void Scale(float scale)
        {
            base.Scale(scale);
            footerLeftColor.A = (byte)(byte.MaxValue * scale);
            footerMiddleColor.A = (byte)(byte.MaxValue * scale);
            footerRightColor.A = (byte)(byte.MaxValue * scale);
        }

        public override void CustomDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (footerLeft != null)
            {
                spriteBatch.DrawString(footerFonts, footerLeft, footerLeftPosition, footerLeftColor,
                    0, footerLeftOrigin, 1, SpriteEffects.None, 0);
            }
            if (footerMiddle != null)
            {
                spriteBatch.DrawString(footerFonts, footerMiddle, footerMiddlePosition, footerMiddleColor,
                    0, footerMiddleOrigin, 1, SpriteEffects.None, 0);
            }
            if (footerRight != null)
            {
                spriteBatch.DrawString(footerFonts, footerRight, footerRightPosition, footerRightColor,
                    0, footerRightOrigin, 1, SpriteEffects.None, 0);
            }
        }
    }
}