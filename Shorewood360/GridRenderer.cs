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
using System.Threading;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GridRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Texture2D backgroundTexture;
        Texture2D individualGrid;
        Texture2D individualGridHighlighted;
        public Texture2D gridTexture;
        public GoalRenderer goalRenderer;
        
        SpriteBatch spriteBatch;
        SpriteFont font;

        public Grid grid;
        public float scale;
        public Vector2 position;
        protected Vector2 internalPosition;
        //public Vector2 levelPosition;
        //public Vector2 lastFiveWordsPosition;
        //public Vector2 scorePosition;
        //public Vector2 scoreWordsPosition;

        //public List<FloatingPoints> floatingPoints;
        public Letter nextLetter = Letter.EmptyLetter;
        public Letter currentLetter = Letter.EmptyLetter;
        public Vector2 realPadding;
        //public string scoreString = "";
        //public string levelString = "";
        //public string lastFiveWordsString = "";
        protected RenderTarget2D gridTarget;
        protected RenderTarget2D rippleTarget;
        Effect rippleEffect;
        public FoundWordAnimationRenderer foundWordAnimator;
        EffectParameter waveParam, distortionParam, centerCoordParam, randomParam;
        bool rippleIsActive = false;

        float rippleStartTime = 0;
        float rippleDuration = 0;
        Vector2 rippleStartLocation = Vector2.Zero;
        Vector2 rippleEndLocation = Vector2.Zero;
        Vector2 centerCoord = new Vector2(0.5f);
        float distortion = 1.0f;
        float divisor = 0.75f;
        float wave = MathHelper.Pi;
        Random rand;
        //float duration = 2000;


        public GridRenderer(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.Visible = false;
            this.Enabled = false;
            
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
            spriteBatch = Shorewood.spriteBatch;//new SpriteBatch(GraphicsDevice);
            int safeAdjustX =  Shorewood.titleSafeArea.X;
            int safeAdjustY =  Shorewood.titleSafeArea.Y;
            //safeAdjustX = (int)MathHelper.Clamp(safeAdjustX, 0, position.X);
            //safeAdjustY = (int)MathHelper.Clamp(safeAdjustY, 0, position.Y);

            //backgroundTexture = Game.Content.Load<Texture2D>("screens\\basebackground");
            individualGrid = Game.Content.Load<Texture2D>("Sprites\\baseIndividualGrid");
            individualGridHighlighted = Game.Content.Load<Texture2D>("Sprites\\baseIndividualGridHighlighted");
            rippleEffect = Game.Content.Load<Effect>("Effects\\ripple");
            font = Game.Content.Load<SpriteFont>("Fonts\\Courier New");

            waveParam = rippleEffect.Parameters["wave"];
            distortionParam = rippleEffect.Parameters["distortion"];
            centerCoordParam = rippleEffect.Parameters["centerCoord"];
            randomParam = rippleEffect.Parameters["random"];
            //centerCoord = new Vector2(((float)backgroundTexture.Width / (float)Shorewood.viewPortRectangle.Width),( (float)backgroundTexture.Height / (float)Shorewood.viewPortRectangle.Height)*0.25f);
            //grid.position.X = safeAdjustX;
            //grid.position.Y = safeAdjustY;
            //levelPosition = scorePosition + new Vector2(0, 25);
            //scoreWordsPosition = levelPosition + new Vector2(0, 25);

            //gridTarget = Shorewood.CloneRenderTarget(GraphicsDevice, 1);
            //rippleTarget = Shorewood.CloneRenderTarget(GraphicsDevice, 1);
            grid.Allocate();
            rand = new Random((int)DateTime.Now.Ticks);

            base.LoadContent();
        }



        private void UpdateRipple(GameTime gameTime)
        {
            float seconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float step = (seconds - rippleStartTime) / rippleDuration;
            if (step < 0.5f)
            {
                divisor = MathHelper.SmoothStep(0.01f, 0.5f, step/0.5f);
            }
            else
            {
                divisor = MathHelper.SmoothStep(0.5f, 0.01f, (step - 0.5f)/0.5f);
            }
            wave = MathHelper.Pi * 10 / divisor;
            waveParam.SetValue(wave);
            distortionParam.SetValue(distortion);
            centerCoordParam.SetValue(centerCoord);
            randomParam.SetValue(1 - 0.05f * (float)rand.NextDouble());
            if (step > 1)
            {
                rippleIsActive = false;
                grid.changed = true;
            }            
        }

        public override void Update(GameTime gameTime)
        {
            if (rippleIsActive)
            {
                UpdateRipple(gameTime);
            }
            //internalPosition = Vector2.Zero;
            base.Update(gameTime);
        }

        public void StartRipple(Vector2 startLocation, Vector2 endLocation, float startTime, float duration)
        {
            if (!rippleIsActive)
            {
                rippleStartTime = startTime;
                rippleDuration = duration;
                rippleStartLocation = startLocation;
                rippleEndLocation = endLocation;
                rippleIsActive = true;
            }
        }

        //private void DrawBackground()
        //{
        //    spriteBatch.Draw(backgroundTexture, Shorewood.viewPortRectangle, null, Color.White);
        //}

        //private void DrawScore()
        //{
        //    spriteBatch.DrawString(font, scoreString, (scorePosition + position) * scale + realPadding, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        //    spriteBatch.DrawString(font, levelString, (levelPosition + position) * scale + realPadding, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        //}

        //private void DrawPoints()
        //{
        //    foreach (var point in floatingPoints)
        //    {
        //        spriteBatch.DrawString(font, point.text, (point.position + position) * scale + realPadding, point.color, 0, Vector2.Zero, point.scale * scale, SpriteEffects.None, 0);
        //    }
        //}


        Vector2 adjust = new Vector2(-4, -5);

        private void DrawGrid()
        {            
            foreach (var row in grid)
            {
                foreach (var letter in row)
                {
                    if ((letter.gridPosition.X == currentLetter.gridPosition.X) && (letter.gridPosition.Y > currentLetter.gridPosition.Y) && (letter == Letter.EmptyLetter)
                        &&Shorewood.normalModeGamePlay.gameplayState == NormalGameplay.NormalGameplayState.CurrentLetterActive)
                    {
                        spriteBatch.Draw(individualGridHighlighted, (letter.position + adjust) * scale + internalPosition, null, Color.White, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                        continue;
                    }
                    spriteBatch.Draw(individualGrid, (letter.position + adjust) * scale + internalPosition, null, Color.White, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                }
            }            
        }

        private void DrawLetters()
        {
            //spriteBatch.Draw(nextLetter.texture, (nextLetter.position + Vector2.UnitY * 100) * scale + position, null, nextLetter.color, nextLetter.rotation, nextLetter.center, scale, SpriteEffects.None, 0);
            foreach (var row in grid)
            {
                foreach (var letter in row)
                {
                    if ((letter.texture != null) && (letter.isVisible))
                    {
                        Color drawColor = Color.White;
                        if (letter.isGlowing)
                        {
                            drawColor = Color.Yellow;
                        }
                        if (letter.isPartOfAValidWord)
                        {
                            if (Shorewood.Is1337Compliant)
                            {
                                spriteBatch.Draw(letter.texture1337, letter.position * scale + internalPosition, null, drawColor, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                            }
                            else
                            {
                                spriteBatch.Draw(letter.texture, letter.position * scale + internalPosition, null, drawColor, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                            }
                        }
                        else
                        {
                            if (Shorewood.Is1337Compliant)
                            {
                                spriteBatch.Draw(letter.texture1337, letter.position * scale + internalPosition, null, Color.Gray, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                            }
                            else
                            {
                                spriteBatch.Draw(letter.texture, letter.position * scale + internalPosition, null, Color.Gray, letter.rotation, letter.center, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                }
            }
        }

        private Texture2D DrawLetters(RenderTarget2D destination, RenderTarget2D original)
        {
            GraphicsDevice.SetRenderTarget(0, destination);
            GraphicsDevice.Clear(Color.TransparentBlack);            
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            DrawGrid();
            DrawLetters();
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(0, original);
            return gridTarget.GetTexture();
        }

        protected Texture2D ApplyEffect(Texture2D texture, RenderTarget2D destination, RenderTarget2D original)
        {
            GraphicsDevice.SetRenderTarget(0, destination);
            GraphicsDevice.Clear(Color.TransparentBlack);  
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            rippleEffect.Begin();
            rippleEffect.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();
            rippleEffect.CurrentTechnique.Passes[0].End();
            rippleEffect.End();
            GraphicsDevice.SetRenderTarget(0, original);
            return rippleTarget.GetTexture();
        }


        //private void DrawToScreen(Texture2D renderedTexture)
        //{
        //    spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
        //    //DrawBackground();
        //    spriteBatch.Draw(renderedTexture, Vector2.Zero, Color.White);
        //    //DrawScore();
        //    spriteBatch.End();
        //}

        private void DrawGrid(bool applyRipple)
        {
            RenderTarget2D temp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            if (grid.changed)
            {
                gridTexture = DrawLetters(gridTarget, temp);
                grid.changed = false;
            }
            if (applyRipple)
            {
                Texture2D tempTexture = gridTexture;
                tempTexture = ApplyEffect(gridTexture, rippleTarget, temp);
                gridTexture = tempTexture;
            } 
           
        }

        public override void Draw(GameTime gameTime)
        {
            DrawGrid(rippleIsActive);
            base.Draw(gameTime);
        }
    }
}