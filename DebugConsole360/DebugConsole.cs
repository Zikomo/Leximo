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
using System.IO;

namespace Debugging
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DebugConsole : Microsoft.Xna.Framework.DrawableGameComponent
    {
        DebugConsoleWriter writer = new DebugConsoleWriter();
        SpriteBatch spriteBatch;
        public SpriteFont font = null;
        public float scale = 1;
        public Vector2 position = new Vector2(200, 200);
        public Queue<string> lines = new Queue<string>();
        public int capacity = 100;
        public int visibleLineCount = 5;
        public Color color = Color.White;
        public DebugConsole(Game game)
            : base(game)
        {
            Console.SetOut(writer);
        }

        public static string ConvertListToString(List<string> list, int lineCount)
        {
            string words = "\r\n";
            int count = list.Count;
            int start = 0;

            if (list.Count > lineCount)
            {
                start = list.Count - lineCount;
            }

            for (int i = start; i < count; i++)
            {
                words += list[i] + "\r\n";
            }
            return words;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        protected void SplitBuilder()
        {
            string currentString = writer.currentLine.ToString();

            if (currentString.LastIndexOf(writer.NewLine) != -1)
            {
                for (int i = currentString.IndexOf(writer.NewLine); i > 0; i = currentString.IndexOf(writer.NewLine))
                {
                    string sub = currentString.Substring(0, i);
                    lines.Enqueue(sub);
                    if (lines.Count > capacity)
                    {
                        lines.Dequeue();
                    }

                    writer.currentLine = writer.currentLine.Remove(0, i + 2);
                    currentString = writer.currentLine.ToString();
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            SplitBuilder();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (font != null)
            {
                string newestString = writer.currentLine.ToString();
                spriteBatch.Begin();
                string words = ConvertListToString(lines.ToList<string>(), visibleLineCount);
                spriteBatch.DrawString(font, words, (position) * scale, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}