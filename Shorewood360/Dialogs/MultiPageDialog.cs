using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IETGames.Shorewood.Input;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood
{
    public class MultiPageDialog:Dialog
    {
        EventHandler<ButtonFireEventArgs> leftShoulder;
        EventHandler<ButtonFireEventArgs> rightShoulder;
        Rectangle page0;
        Rectangle page1;
        Vector2 position;
        Vector2 destination;
        Vector2 previousPosition;
        bool isMoving = false;
        bool isMovingRight = false;
        bool isMovingLeft = false;
        TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan time = new TimeSpan();
        Rectangle sourceRectangle0;
        Rectangle destinationRectangle0;
        Rectangle sourceRectangle1;
        Rectangle destinationRectangle1;
        protected bool isSinglePage = false;
        protected int pageCount = 1;
        protected Texture2D singlePage;
        protected Texture2D[] renderedPages;
        public Texture2D background;
        public Texture2D overlay;
        public Texture2D dPad;
        protected int index = 0;
        protected bool resetOnDeactivate = false;

        public MultiPageDialog(StringBuilder dialogTitle, Texture2D background,Vector2 size)
            : base(dialogTitle, size )
        {
            leftShoulder = new EventHandler<ButtonFireEventArgs>(OnLeftShoulder);
            rightShoulder = new EventHandler<ButtonFireEventArgs>(OnRightShoulder);         
            this.background = background;
            dPad = Shorewood.Content.Load<Texture2D>(@"Sprites\xboxControllerDPad");
            AllowPurchase = false;
        }

        protected virtual void MoveRightSinglePage(GameTime gameTime)
        {
            if (!isMoving)
            {
                if (index < pageCount - 1)
                {
                    time = TimeSpan.Zero;
                    destination.X += bounds.Width;
                    //destination.Y = position.Y;
                    previousPosition = position;
                    index++;                    
                    isMoving = true;
                    if (Shorewood.dialogManager.IsActive)
                    {
                        Shorewood.scroll.Play();
                    }
                }
            }
        }

        protected virtual void MoveRightMultiPage(GameTime gameTimeStart)
        {
            if (!isMoving)
            {
                if (index < renderedPages.Length - 1)
                {
                    page0 = bounds;
                    page1 = new Rectangle(bounds.Right, bounds.Y, bounds.Width, bounds.Height);
                    time = TimeSpan.Zero;
                    destination.X = bounds.Left - bounds.Width;
                    destination.Y = bounds.Y;
                    previousPosition = new Vector2(bounds.X, bounds.Y);
                    index++;
                    isMovingRight = true;
                    isMoving = true;
                    if (Shorewood.dialogManager.IsActive)
                    {
                        Shorewood.scroll.Play();
                    }
                }
            }
        }

        protected virtual void MoveLeftSinglePage(GameTime gameTime)
        {
            if (!isMoving)
            {
                if (index > 0)
                {
                    time = TimeSpan.Zero;
                    destination.X -= bounds.Width;
                    //destination.Y = 0;
                    previousPosition = position;
                    index--;
                    isMoving = true;
                    if (Shorewood.dialogManager.IsActive)
                    {
                        Shorewood.scroll.Play();
                    }
                }
            }
        }

        protected virtual void MoveLeftMultiPage(GameTime gameTimeStart)
        {
            if (!isMoving)
            {
                if (index > 0)
                {
                    page0 = bounds;
                    page1 = new Rectangle(bounds.X - bounds.Width, bounds.Y, bounds.Width, bounds.Height);
                    destination.X = bounds.X;
                    destination.Y = bounds.Y;
                    time = TimeSpan.Zero;
                    previousPosition = new Vector2(page1.X, bounds.Y);
                    index--;
                    isMovingLeft = true;
                    isMoving = true;
                    if (Shorewood.dialogManager.IsActive)
                    {
                        Shorewood.scroll.Play();
                    }
                }
            }
        }

        protected virtual void MoveRight(GameTime gameTimeStart)
        {
            if (isSinglePage)
            {
                MoveRightSinglePage(gameTimeStart);
            }
            else
            {
                MoveRightMultiPage(gameTimeStart);
            }
        }

        protected virtual void MoveLeft(GameTime gameTimeStart)
        {
            if (isSinglePage)
            {
                MoveLeftSinglePage(gameTimeStart);
            }
            else
            {
                MoveLeftMultiPage(gameTimeStart);
            }
        }

        protected override void OnActivated(GameTime gameTime)
        {
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickLeft, leftShoulder);            
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickRight, rightShoulder);
            Shorewood.inputHandler.AddEvent(Buttons.DPadLeft, leftShoulder);
            Shorewood.inputHandler.AddEvent(Buttons.DPadRight, rightShoulder);
            base.OnActivated(gameTime);
        }

        protected override void OnDeactivated(GameTime gameTime)
        {
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickLeft, leftShoulder);            
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickRight, rightShoulder);
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadLeft, leftShoulder);
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadRight, rightShoulder);
            if (resetOnDeactivate)
            {
                previousPosition = Vector2.Zero;
                destination = Vector2.Zero;
                index = 0;
                position = Vector2.Zero;
            }
            base.OnDeactivated(gameTime);
        }

        public virtual void UpdateSinglePage(GameTime gameTime)
        {
            if (isMoving)
            {
                time += gameTime.ElapsedGameTime;
                position = Vector2.SmoothStep(previousPosition, destination, (float)time.TotalMilliseconds / (float)duration.TotalMilliseconds);
                if (position == destination)
                {
                    isMoving = false;
                }
            }
        }

        public virtual void UpdateMultiPage(GameTime gameTime)
        {
            if (isMoving)
            {
                time += gameTime.ElapsedGameTime;
                position = Vector2.SmoothStep(previousPosition, destination, (float)time.TotalMilliseconds / (float)duration.TotalMilliseconds);
                if (position == destination)
                {
                    isMovingRight = false;
                    isMovingLeft = false;
                    isMoving = false;
                }
                else
                {
                    page0 = new Rectangle((int)position.X, bounds.Y, bounds.Width, bounds.Height);
                    page1 = new Rectangle((int)position.X + bounds.Width, bounds.Y, bounds.Width, bounds.Height);
                    Rectangle intersect0 = Rectangle.Intersect(page0, bounds);
                    Rectangle intersect1 = Rectangle.Intersect(page1, bounds);
                    destinationRectangle0 = new Rectangle(bounds.X, bounds.Y, intersect0.Width, intersect0.Height);
                    destinationRectangle1 = new Rectangle((int)position.X + bounds.Width, bounds.Y, bounds.Width, bounds.Height);
                    sourceRectangle0 = new Rectangle(page0.Width - intersect0.Width, 0, intersect0.Width, intersect0.Height);
                    sourceRectangle1 = new Rectangle(0, 0, intersect1.Width, intersect1.Height);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isSinglePage)
            {
                UpdateSinglePage(gameTime);
            }
            else
            {
                UpdateMultiPage(gameTime);
            }
            base.Update(gameTime);
        }

        protected virtual void DrawMultiPage(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (isMovingRight)
            {
                Rectangle intersect0 = Rectangle.Intersect(page0, bounds);
                Rectangle intersect1 = Rectangle.Intersect(page1, bounds);
                spriteBatch.Draw(renderedPages[index - 1], destinationRectangle0, sourceRectangle0, Color.White);
                spriteBatch.Draw(renderedPages[index], position + Vector2.UnitX * bounds.Width, sourceRectangle1, Color.White);
            }
            else if (isMovingLeft)
            {
                Rectangle intersect0 = Rectangle.Intersect(page0, bounds);
                Rectangle intersect1 = Rectangle.Intersect(page1, bounds);
                spriteBatch.Draw(renderedPages[index], destinationRectangle0, sourceRectangle0, Color.White);
                spriteBatch.Draw(renderedPages[index + 1], position + Vector2.UnitX * bounds.Width, sourceRectangle1, Color.White);
            }
            else
            {
                spriteBatch.Draw(renderedPages[index], bounds, new Rectangle(0, 0, bounds.Width, bounds.Height), Color.White);
            }
        }

        protected virtual void DrawSinglePage(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(singlePage, bounds, new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height), Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (background != null)
            {
                spriteBatch.Draw(background, center - new Vector2(background.Width / 2, background.Height / 2), Color.White);
            }

            if (isSinglePage)
            {
                DrawSinglePage(spriteBatch, gameTime);
            }
            else
            {
                DrawMultiPage(spriteBatch, gameTime);
            }

            if (overlay != null)
            {
                spriteBatch.Draw(overlay, center - new Vector2(overlay.Width / 2, overlay.Height / 2), Color.White);
            }
            StringBuilder builder;
            Vector2 builderBounds;
            if (index > 0)
            {
                builder = Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuPrevious];
                builderBounds = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(builder);
                spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont], builder,
                    new Vector2(bounds.Left + bounds.Width/2 - dPad.Width/2 - builderBounds.X, bounds.Bottom + 20), Color.White);
            }
            if (index < 4)
            {
                builder = Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuNext];
                builderBounds = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(builder);
                spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont], builder,
                    new Vector2(bounds.Right - bounds.Width / 2 + dPad.Width / 2, bounds.Bottom + 20), Color.White);
            }
            builder = Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogBack];
            builderBounds = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(builder);
            spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont], builder,
                new Vector2(bounds.Right - builderBounds.X - 50, bounds.Bottom + 20), Color.White);

            spriteBatch.Draw(dPad, new Vector2(bounds.Left + bounds.Width / 2 - dPad.Width / 2, bounds.Bottom + 20- builderBounds.Y / 2), Color.White);
            CustomDraw(spriteBatch, gameTime);
            
        }


        protected virtual void OnLeftShoulder(object sender, ButtonFireEventArgs args)
        {
            MoveLeft(args.gameTime);
        }

        protected virtual void OnRightShoulder(object sender, ButtonFireEventArgs args)
        {
            MoveRight(args.gameTime);
        }

    }
}