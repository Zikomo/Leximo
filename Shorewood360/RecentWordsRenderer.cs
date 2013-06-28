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
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using IETGames.Shorewood.Physics;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RecentWordsRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Queue<WordBuilder> recentWords;
        public float scale = 1;
        public Vector2 position = new Vector2(0, 25);
        
        public int capacity;
        public Vector2 startOffsetPosition;
        private Texture2D background;
        private RenderTarget2D target;
        private Texture2D renderedTexture;
        protected Body wordBoxBody;
        protected Vector2 wordBoxLowestPosition;
        protected Vector2 wordBoxStartPosition;
        protected Matrix translationMatrix;
        Chain wordBoxChain;
        int chainMass = 100000;
        protected int links = 17;
        Vector2 wordBoxOrigin;

        public bool Changed
        {
            get;
            set;
        }


        public RecentWordsRenderer(Game game, int capacity)
            : base(game)
        {
            this.capacity = capacity;
            recentWords = new Queue<WordBuilder>(capacity);
            Enabled = false;
            Visible = false;
        }

        public void Add(WordBuilder builder)
        {
            if (builder.letters.Count > 5)
            {
                for (int i = 0; i < builder.letters.Count; i++)
                {
                    Letter letter = builder.letters[i];
                    letter.scale *= 0.80f;
                    builder.letters[i] = letter;
                }
            }
            recentWords.Enqueue(builder);
            if (recentWords.Count > capacity)
            {
                WordBuilder oldBuilder = recentWords.Dequeue();
            }
            wordBoxBody.ApplyImpulse(new Vector2(chainMass*50,chainMass*50));
            Changed = true;
        }

        public Vector2 Position
        {
            get
            {
                return wordBoxBody.Position;
            }
        }

        public void Clear()
        {
            recentWords.Clear();
        }

        protected void SetupPhysics()
        {
            wordBoxStartPosition = new Vector2(-150, -75);
            wordBoxLowestPosition = new Vector2(Shorewood.titleSafeArea.X, 200);
            wordBoxBody = BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, background.Width, background.Height, 10000);
            wordBoxChain = new Chain(Shorewood.physicsEngine, (int)(links), wordBoxStartPosition, 10, chainMass);
            wordBoxChain.AttachEnd(new BodyNode(wordBoxBody, background, false), 0);
            wordBoxChain.AttachEnd(new BodyNode(BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, 10, 10, wordBoxChain.chainMass), null), 0);
            for (int i = 1; i < links; i++)
            {
                wordBoxChain.AttachEnd(new BodyNode(BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, 10, 10, wordBoxChain.chainMass), null), 0);
            }
            wordBoxChain.links[wordBoxChain.links.Count - 1].body.IsStatic = true;
        }
        

        protected override void LoadContent()
        {
            background = Shorewood.Content.Load<Texture2D>("screens\\recentlyfoundwords");
            target = new RenderTarget2D(GraphicsDevice, background.Width, background.Height, 1, background.Format);
            wordBoxOrigin = new Vector2(background.Width / 2, background.Height / 2);
            SetupPhysics();
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Reset()
        {
            recentWords.Clear();
            Changed = true;
            wordBoxChain.Reset();
            
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                Reset();
            }
            base.OnEnabledChanged(sender, args);
        }

        public override void Update(GameTime gameTime)
        {
            wordBoxBody.Position = Vector2.Clamp(wordBoxBody.Position, wordBoxStartPosition, wordBoxLowestPosition);
            translationMatrix = Shorewood.UpdateTranslationMatrix(wordBoxBody.Position + Vector2.UnitX * Shorewood.titleSafeArea.X);
            base.Update(gameTime);
        }

        //public void DrawToTexture(GameTime gameTime)
        //{
        //    RenderTarget2D temp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
        //    GraphicsDevice.SetRenderTarget(0, target);
        //    Shorewood.spriteBatch.Begin();
        //    GraphicsDevice.Clear(Color.TransparentBlack);
        //    DrawBackground();
        //    DrawLetters();
        //    Shorewood.spriteBatch.End();
        //    GraphicsDevice.SetRenderTarget(0, temp);
        //    renderedTexture = target.GetTexture();
        //    Changed = false;
        //}

        public void DrawToScreen(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(renderedTexture, wordBoxBody.Position + Vector2.UnitX * Shorewood.titleSafeArea.X,
                null, Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            Shorewood.spriteBatch.End();
        }

        private void DrawBackground()
        {
            Shorewood.spriteBatch.Draw(background, Vector2.Zero, Color.White);
        }

        public override void Draw(GameTime gameTime)
        {
            //if (Changed)
            //{
            //    DrawToTexture(gameTime);
            //}
            Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, translationMatrix);
            DrawBackground();
            DrawLetters();
            Shorewood.spriteBatch.End();
            //DrawToScreen(gameTime);
            base.Draw(gameTime);
        }

        private void DrawLetters()
        {
            Vector2 offset = new Vector2();            
            for (int i = recentWords.Count - 1; i >= 0; i--)
            {
                
                offset.X = 0;
                WordBuilder word = recentWords.ElementAt(i);
                offset.Y += word.letters[0].height * word.letters[0].scale;
                if (!word.isReversed)
                {                    
                    foreach (var letter in word.letters)
                    {
                        offset.X += word.letters[0].width * word.letters[0].scale;
                        if (Shorewood.Is1337Compliant)
                        {
                            Shorewood.spriteBatch.Draw(letter.texture1337, offset * scale, null, Color.White, 0, Vector2.Zero, letter.scale * scale, SpriteEffects.None, 0);
                        }
                        else
                        {
                            Shorewood.spriteBatch.Draw(letter.texture, offset * scale, null, Color.White, 0, Vector2.Zero, letter.scale * scale, SpriteEffects.None, 0);
                        }
                    }
                }
                else
                {
                    for (int j = word.letters.Count - 1; j >= 0; j--)
                    {
                        offset.X += word.letters[0].width * word.letters[0].scale;
                        if (Shorewood.Is1337Compliant)
                        {
                            Shorewood.spriteBatch.Draw(word.letters[j].texture1337, offset * scale, null, Color.White, 0, Vector2.Zero, word.letters[j].scale * scale, SpriteEffects.None, 0);
                        }
                        else
                        {
                            Shorewood.spriteBatch.Draw(word.letters[j].texture, offset * scale, null, Color.White, 0, Vector2.Zero, word.letters[j].scale * scale, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
    }
}