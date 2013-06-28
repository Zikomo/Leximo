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
using Particle3DSample;


namespace IETGames.Shorewood
{

    public struct FoundWordHelper
    {

        public WordBuilder Key
        {
            get;
            set;
        }

        public TimeSpan Value
        {
            get;
            set;
        }
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FoundWordAnimationRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Dictionary<WordBuilder, TimeSpan> words = new Dictionary<WordBuilder, TimeSpan>(10);
        List<FoundWordHelper> words = new List<FoundWordHelper>(10);
        //Dictionary<float, LightningBolt> bolts = new Dictionary<float, LightningBolt>(10);
        List<FoundWordHelper> garbage = new List<FoundWordHelper>(10);
        //List<float> garbageBolts = new List<float>();
        //List<FloatingPoints> points = new List<FloatingPoints>();
        public float duration = 2000;
        public Vector2 destination = Vector2.Zero;
        public float destinationScale = 0.25f;
        public float startingScale = 1;
        public float scale = 1;
        public Vector2 startPositionOffset;
        private SpriteBatch spriteBatch;
        public RecentWordsRenderer recentWords;
        public SpriteFont pointsFont; 
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix worldMatrix;
        Vector2 titleSafeVector;

        
        public FoundWordAnimationRenderer(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            pointsFont = Shorewood.fonts[FontTypes.FloatingPointsFont];
            spriteBatch = Shorewood.spriteBatch;//new SpriteBatch(GraphicsDevice);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width /5, GraphicsDevice.Viewport.Height / 5, -5000, 5000);
            worldMatrix = Matrix.Identity;
            startPositionOffset = Vector2.Zero;
            startingScale *= scale;
            titleSafeVector = new Vector2(Shorewood.titleSafeArea.X, 0);
            base.LoadContent();
        }
        Random rand = new Random();
        public void Add(WordBuilder builder, TimeSpan timeSpan)
        {
            lock (words)
            {
                destination = Shorewood.recentWords.Position;
                for (int i = 0; i < builder.letters.Count; i++)
                {
                    Letter letter = builder.letters[i];
                    letter.startPosition *= Shorewood.normalGameplayRenderer.scale;
                    builder.letters[i] = letter;
                }
                FloatingPoints point = new FloatingPoints((builder.letters[0].startPosition), new Vector2(rand.Next(0, 1200), -20), builder.points);
                Shorewood.particleSystem.TriggerProjectile(point);
                CalculateDestinations(builder);
                Shorewood.pew.Play();
                //if (!words.Keys.Contains(builder))
                bool containsWord = false;
                for (int i = 0; i < words.Count; i++)
                {
                    if (words[i].Key == builder)
                    {
                        containsWord = true;
                        break;
                    }
                }
                if (!containsWord)
                {
                    FoundWordHelper word = new FoundWordHelper();
                    word.Key = builder;
                    word.Value = TimeSpan.Zero;
                    words.Add(word);
                }
                Visible = true;
                Enabled = true;
            }
        }

        

        private void CalculateDestinations(WordBuilder builder)
        {
            for (int i = 0; i < builder.letters.Count; i++)
            {
                Letter letter = builder.letters[i];
                if (builder.isReversed)
                {
                    letter.destination = (destination + titleSafeVector ) + (Vector2.UnitX * ((builder.letters.Count - 1) - i) * letter.width) * destinationScale * destinationScale;
                }
                else
                {
                    letter.destination = (destination + titleSafeVector ) + (Vector2.UnitX * i * letter.width) * destinationScale * destinationScale;
                }
                builder.letters[i] = letter;
            }
        }

        private void CleanUpWords()
        {
            lock (words)
            {
                foreach (var word in garbage)
                {
                    words.Remove(word);
                    recentWords.Add(word.Key);
                }
            }
            //garbageBolts.Clear();
            garbage.Clear();
        }

        private Vector3 Unproject(Vector2 location)
        {
            return GraphicsDevice.Viewport.Unproject(new Vector3(location, 0), projectionMatrix, viewMatrix, worldMatrix);
        }

        private void UpdateWords(GameTime gameTime)
        {
            lock (words)
            {
                bool hasGarbage = false;
                //foreach (var wordEntry in words)
                for (int i = 0; i < words.Count; i++)
                {
                    FoundWordHelper wordEntry = words[i];
                    int count = 0;
                    float step = (float)((wordEntry.Value.TotalMilliseconds) / duration);
                    wordEntry.Value += gameTime.ElapsedGameTime;
                    for (int j = 0; j < wordEntry.Key.letters.Count; j++)
                    {
                        Letter letter = wordEntry.Key.letters[j];                       
                        step = MathHelper.Clamp(step, 0, 1);
                        if (step < 1)
                        {
                            letter.scale = MathHelper.SmoothStep(letter.startScale, destinationScale, step);
                            letter.position = Vector2.SmoothStep(letter.startPosition, letter.destination, step);
                            wordEntry.Key.letters[j] = letter;
                        }
                        else
                        {
                            hasGarbage = true;
                        }
                        count++;
                    }
                    if (hasGarbage)
                    {
                        garbage.Add(wordEntry);
                        //garbageBolts.Add(wordEntry.Value);
                    }
                    words[i] = wordEntry;
                    hasGarbage = false;
                    
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            destination = Shorewood.recentWords.Position;
            if (words.Count > 0)
            { 
                UpdateWords(gameTime);             
                CleanUpWords();                
            }
            base.Update(gameTime);
        }

        public  Vector2 Project(Vector3 location)
        {
            Vector3 twoDimensional = GraphicsDevice.Viewport.Project(location, projectionMatrix, viewMatrix, worldMatrix);
            return new Vector2(twoDimensional.X, twoDimensional.Y);
        }



        private void DrawWord(WordBuilder word)
        {            
            foreach (var letter in word.letters)
            {
                if (Shorewood.Is1337Compliant)
                {
                    spriteBatch.Draw(letter.texture1337, (letter.position), null, Color.White, 0, Vector2.Zero, letter.scale * scale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(letter.texture, (letter.position), null, Color.White, 0, Vector2.Zero, letter.scale * scale, SpriteEffects.None, 0);
                }
            }
        }

        public int WordsInTransit
        {
            get
            {
                return words.Count;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            lock (words)
            {
                foreach (var wordEntry in words)
                {
                    DrawWord(wordEntry.Key);
                    //DrawPoints();
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}