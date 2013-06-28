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
using System.Text;
using IETGames.Shorewood.Localization;
using IETGames.Shorewood.Utilities;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GoalRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public class GoalBox
        {
            public Texture2D backGroundTexture;
            public Texture2D completedTexture;
            public Rectangle bounds;
            public Rectangle drawBounds;
            public Rectangle completedBounds;
            public Rectangle overlayBounds;
            public StringBuilder goalText;
            public StringBuilder goalName;
            public Color goalColor;
            public Color nameColor;
            public Vector2 boxPosition;
            public int padding;
            public bool IsFading;
            public bool IsDiscarding;
            public bool IsDiscarded = false;
            protected TimeSpan fadeDuration = new TimeSpan(0, 0, 0,0,500);
            protected TimeSpan fadeTime = TimeSpan.Zero;
            public float currentScale = 1;
            public float rotation = 0;
            public float alpha = 0;

            private Matrix rotationMatrix = Matrix.Identity;
            private Matrix scalingMatrix = Matrix.Identity;
            public Matrix transformationMatrix = Matrix.Identity;
            private void UpdateRotationMatrix( Vector2 origin, float radians)
            {
                // Translate sprites to center around screen (0,0), rotate them, and
                // translate them back to their original positions
                Vector3 matrixorigin = new Vector3(origin, 0);
                rotationMatrix = Matrix.CreateRotationZ(radians);

                scalingMatrix = Matrix.CreateScale(currentScale);
                transformationMatrix = Matrix.CreateTranslation(-matrixorigin) * rotationMatrix * scalingMatrix * Matrix.CreateTranslation(matrixorigin);
            }
            
            
            //StringBuilder helperBuilder;

            public GoalBox(Texture2D backGroundTexture, Texture2D completedTexture, Rectangle bounds)
            {
                padding = 10;
                this.backGroundTexture = backGroundTexture;
                this.completedTexture = completedTexture;
                this.bounds = bounds;
                this.completedBounds = Rectangle.Empty;
                this.drawBounds = Rectangle.Empty;
                this.overlayBounds = Rectangle.Empty;
                goalText = new StringBuilder(512);
                goalName = new StringBuilder(512);
                goalColor = Color.White;
                nameColor = Color.Wheat;
                boxPosition = Vector2.Zero;
                
            }

            public void UpdateGoal(IGoal goal, int Y)
            {
                Rectangle paddedBounds = bounds;
                paddedBounds.Inflate(-padding, 0);
                goalText.Remove(0, goalText.Length);
                WordWrapper.WrapWord(goal.GoalText, goalText, Shorewood.fonts[FontTypes.GoalFont], paddedBounds, 1);
                Vector2 stringBounds = Shorewood.fonts[FontTypes.GoalFont].MeasureString(goalText);
                drawBounds = new Rectangle(paddedBounds.X, Y, (int)(paddedBounds.Width), (int)(stringBounds.Y));
                completedBounds = new Rectangle(drawBounds.X, Y, (int)(drawBounds.Width * goal.GoalStatus), drawBounds.Height);
                overlayBounds = drawBounds;
                overlayBounds.Inflate(15, 9);
                boxPosition = new Vector2(drawBounds.X, drawBounds.Y);
            }

            public void UpdateFading(GameTime gameTime)
            {
                fadeTime += gameTime.ElapsedGameTime;
                rotation = MathHelper.SmoothStep(0, MathHelper.TwoPi, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds);
                currentScale = MathHelper.SmoothStep(10, 1, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds);
                alpha = MathHelper.SmoothStep(0, 1, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds) * 255;
                Vector2 center = new Vector2(drawBounds.Width / 2, drawBounds.Height / 2) ;
                UpdateRotationMatrix( boxPosition + center, rotation);
                if (fadeTime.TotalMilliseconds > fadeDuration.TotalMilliseconds)
                {
                    IsFading = false;
                }
            }

            public void UpdateDiscarding(GameTime gameTime)
            {
                fadeTime += gameTime.ElapsedGameTime;
                rotation = MathHelper.SmoothStep(0, MathHelper.TwoPi, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds);
                currentScale = MathHelper.SmoothStep(1, 0, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds);
                alpha = MathHelper.SmoothStep(1, 0, (float)fadeTime.TotalMilliseconds / (float)fadeDuration.TotalMilliseconds) * 255;
                Vector2 center = new Vector2(drawBounds.Width / 2, drawBounds.Height / 2);
                UpdateRotationMatrix(boxPosition + center, rotation);
                if (fadeTime.TotalMilliseconds > fadeDuration.TotalMilliseconds)
                {
                    IsDiscarding = false;
                    IsDiscarded = true;
                }
            }

            public void Reset()
            {
                fadeTime = TimeSpan.Zero;
                IsDiscarding = false;
                IsDiscarded = false;
                IsFading = true;
            }

            public void Discard()
            {
                fadeTime = TimeSpan.Zero;
                IsDiscarding = true;
                IsDiscarded = false;
                IsFading = false;
            }
        }

        public Vector2 position = Vector2.Zero;
        private Vector2 goalBoxStartPosition = Vector2.Zero;
        public Rectangle bounds;
        public Rectangle drawBounds;
        //public GoalBox[] goalBoxes = new GoalBox[20];
        public float goalGap = 20;
        public LevelManager levelManager;
        private StringBuilder helperBuilder = new StringBuilder(512);
        private StringBuilder drawBuilder = new StringBuilder(512);
        //private int currentLevel = 0;
        Texture2D goalBaseBackground;
        //Texture2D goalBaseHighlight;
        protected bool renderGoals = true;
        
        public GoalRenderer(Game game)
            : base(game)
        {
            Visible = false;
            Enabled = false;           
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        Texture2D backGroundTexture;
        Texture2D completedTexture;
        protected override void LoadContent()
        {
            backGroundTexture = new Texture2D(GraphicsDevice, 1, 1);
            completedTexture = new Texture2D(GraphicsDevice, 1, 1);

            goalBaseBackground = Shorewood.Content.Load<Texture2D>(@"screens\baseGoalBox");            

            Color[] backGroundPixels = { new Color(Color.White, 0.25f) };
            Color[] completedPixels = { Color.LightGreen };

            backGroundTexture.SetData<Color>(backGroundPixels);
            completedTexture.SetData<Color>(completedPixels);
            
            if (bounds == Rectangle.Empty)
            {
                bounds = new Rectangle((int)(position.X), (int)(position.Y), ((int)Shorewood.titleSafeArea.Right) - (int)position.X, (int)Shorewood.titleSafeArea.Height - (int)position.Y - 20);
            }            

            goalBoxStartPosition = Vector2.UnitY * (position.Y + 105);
            base.LoadContent();
        }

        bool forceUpdate = false;
        public void ForceUpdateOnce()
        {
            forceUpdate = true;
        }

        public GoalBox GetGoalBox()
        {
            return new GoalBox(backGroundTexture, completedTexture, bounds);
        }

        private bool ShouldUpdateGoals()
        {            
            foreach (var goal in levelManager.CurrentLevelGoals)
            {
                if (goal.HasUpdated)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            if (Visible)
            {
                ForceUpdateOnce();
            }
            base.OnVisibleChanged(sender, args);
        }



        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            int index = 0;
            float previousGoalBottom = 0;
            for (int i = 0; i < levelManager.CurrentLevelGoals.Count; i++)
            {
                if (levelManager.CurrentLevelGoals[i].GoalBox.IsDiscarded)
                {
                    levelManager.CurrentLevelGoals.RemoveAt(i);
                    forceUpdate = true;
                    i = 0;
                }
            }
            if (ShouldUpdateGoals()||forceUpdate)
            {
                helperBuilder.Remove(0, helperBuilder.Length);
                drawBuilder.Remove(0, drawBuilder.Length);
                //currentLevel = levelManager.currentLevel;
                foreach (var goal in levelManager.CurrentLevelGoals)
                {
                    //GoalBox goalBox = goalBoxes[index];
                    if (index == 0)
                    {
                        goal.GoalBox.UpdateGoal(goal, (int)goalBoxStartPosition.Y);
                    }
                    else
                    {
                        goal.GoalBox.UpdateGoal(goal, ((int)previousGoalBottom + (int)goalGap));
                    }
                    previousGoalBottom = goal.GoalBox.drawBounds.Bottom;
                    //goal.GoalBox = goal.GoalBox;
                    goal.HasUpdated = false;
                    index++;                    
                }
                forceUpdate = false;
            }
            foreach (var goal in levelManager.CurrentLevelGoals)
            {
                if (goal.GoalBox.IsFading)
                {
                    goal.GoalBox.UpdateFading(gameTime);                    
                }
                if (goal.GoalBox.IsDiscarding)
                {
                    goal.GoalBox.UpdateDiscarding(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public void DrawBackground()
        {
            Shorewood.spriteBatch.Draw(goalBaseBackground, bounds, Color.White);
        }

        //public void DrawHighlight()
        //{
        //    Shorewood.spriteBatch.Draw(goalBaseHighlight, bounds, Color.White);
        //}
       
        public void DrawGoals()
        {            
            for (int i = 0; i < levelManager.CurrentLevelGoals.Count; i++)
            {
                GoalBox goalBox = levelManager.CurrentLevelGoals[i].GoalBox;
                if (!goalBox.IsFading && !goalBox.IsDiscarding && !goalBox.IsDiscarded)
                {
                    Shorewood.spriteBatch.Draw(goalBox.backGroundTexture, goalBox.drawBounds, Color.White);
                    Shorewood.spriteBatch.Draw(goalBox.completedTexture, goalBox.completedBounds, Color.White);
                    Shorewood.spriteBatch.Draw(goalBaseBackground, goalBox.overlayBounds, Color.White);
                    Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.GoalFont], goalBox.goalText, goalBox.boxPosition, Color.DarkSlateGray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }                
            }
        }

        public void DrawFadingInGoals()
        {
            for (int i = 0; i < levelManager.CurrentLevelGoals.Count; i++)
            {
                GoalBox goalBox = levelManager.CurrentLevelGoals[i].GoalBox;
                if (goalBox.IsFading)
                {
                    Color colorDraw = new Color(Color.White, (byte)goalBox.alpha);
                    
                    Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, goalBox.transformationMatrix);
                    Shorewood.spriteBatch.Draw(goalBox.backGroundTexture, goalBox.drawBounds, colorDraw);
                    Shorewood.spriteBatch.Draw(goalBox.completedTexture, goalBox.completedBounds, colorDraw);
                    Shorewood.spriteBatch.Draw(goalBaseBackground, goalBox.overlayBounds, colorDraw);
                    Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.GoalFont], goalBox.goalText, goalBox.boxPosition, new Color(Color.DarkSlateGray, (int)goalBox.alpha), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    Shorewood.spriteBatch.End();
                }
            }
        }

        private void DrawDiscardingInGoals()
        {
            for (int i = 0; i < levelManager.CurrentLevelGoals.Count; i++)
            {
                GoalBox goalBox = levelManager.CurrentLevelGoals[i].GoalBox;
                if (goalBox.IsDiscarding)
                {
                    Color colorDraw = new Color(Color.White, (byte)goalBox.alpha);
                    Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, goalBox.transformationMatrix);
                    Shorewood.spriteBatch.Draw(goalBox.backGroundTexture, goalBox.drawBounds, colorDraw);
                    Shorewood.spriteBatch.Draw(goalBox.completedTexture, goalBox.completedBounds, colorDraw);
                    Shorewood.spriteBatch.Draw(goalBaseBackground, goalBox.overlayBounds, colorDraw);
                    Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.GoalFont], goalBox.goalText, goalBox.boxPosition, new Color(Color.DarkSlateGray, (int)goalBox.alpha), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    Shorewood.spriteBatch.End();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawFadingInGoals();
            Shorewood.spriteBatch.Begin();            
            if (renderGoals)
            {
                DrawGoals();
            }            
            Shorewood.spriteBatch.End();
            DrawDiscardingInGoals();
            base.Draw(gameTime);
        }


    }
}