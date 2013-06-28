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
using IETGames.Shorewood;
using Fluids;

namespace NinthPlanetGames
{
    public struct Floater
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Age;
        public float Scale;
        Random random;
        

        public Floater(Vector2 position)
        {
            random = new Random();
            Position = position;
            Velocity = Vector2.Zero;
            Color = Color.White;
            Age = (float)random.NextDouble() * 50;
            Scale = 1;
        }

        public Floater(Vector2 position, Vector2 velocity)
        {
            random = new Random();
            Position = position;
            Velocity = velocity;
            Color = Color.White;
            Age = (float)random.NextDouble()*50;
            Scale = 1;
        }

        public Floater(Vector2 position, Vector2 velocity, Color color)
        {
            random = new Random();
            Position = position;
            Velocity = velocity;
            Color = color;
            Age = (float)random.NextDouble() * 50;
            Scale = 1;
        }
    }

    public class SplashScreen : DrawableGameComponent
    {
        public int duration = 5000;
        public bool animationFinished = false;
        public bool animationStarted = false;
        public int maxFloaters = 1000;
        public float logoSwapDuration = 1000;

        Effect hotShit;
        Floater[] floaters;
        Random random = new Random();
        Texture2D mask;
        Texture2D snowTexture;
        Texture2D backgroundTexture;
        Texture2D logo;
        Texture2D backGroundColorTexture;
        Color backgroundColor = Color.Black;
        Vector2 logoEndPosition = new Vector2(-2280, 0);
        Vector2 logoDrawPosition = Vector2.Zero;
        float logoSwapStartTime = 0;
        
        float startTime = 0;
        int floatersAlive = 0;
        
        public SplashScreen(Game game)
            : base(game)
        {
            floaters = new Floater[maxFloaters];
        }



        protected override void LoadContent()
        {
            snowTexture = Shorewood.Content.Load<Texture2D>("Sprites\\sparkleF5");
            backgroundTexture = Shorewood.Content.Load<Texture2D>("screens\\baseBackgroundOverlay");
            logo = Shorewood.Content.Load<Texture2D>("screens\\NinthPlanetSplashTwo");
            hotShit = Shorewood.Content.Load<Effect>("Effects\\HotShit");
            mask = Shorewood.Content.Load<Texture2D>("screens\\mask");
            hotShit.Parameters["mask"].SetValue(mask);
            backGroundColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] black = { Color.White };
            backGroundColorTexture.SetData<Color>(black);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (startTime == 0)
            {
                startTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
            float time = MathHelper.Clamp(((float)gameTime.TotalGameTime.TotalMilliseconds- startTime) / (duration), 0, 1);
            Vector2 location = Vector2.SmoothStep(Vector2.Zero, new Vector2(1280, 720), time);
            Shorewood.fluidSolver.Trigger((int)location.X, (int)location.Y, 1);                
            for (int i = 0; (i < 10) && (floatersAlive < floaters.Length); i++)
            {
                floaters[floatersAlive] = SpawnFuzz(0);
                floatersAlive++;
            }
            for (int i = 0; i < floaters.Length; i++)
            {
                Floater floater = floaters[i];
                floater.Velocity += Shorewood.fluidSolver.GetVelocity((int)floater.Position.X, (int)floater.Position.Y);
                floater.Position += floater.Velocity;
                floater.Scale = (float)Math.Sin(floater.Age);
                floater.Age+=(float)random.NextDouble()/5f;
                floaters[i] = floater;               
            }
            if (time > 0.90)
            {
                if (logoSwapStartTime == 0)
                {
                    logoSwapStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    logoSwapDuration = (float)(startTime + duration) - logoSwapStartTime;
                }
                logoDrawPosition = Vector2.SmoothStep(Vector2.Zero, logoEndPosition, ((float)gameTime.TotalGameTime.TotalMilliseconds - logoSwapStartTime) / logoSwapDuration);
            }
            if (time == 1)
            {
                animationFinished = true;
            }
            backgroundColor = new Color(Vector3.Lerp(Color.Black.ToVector3(), Color.White.ToVector3(), time));
            base.Update(gameTime);
        }

        public void DrawBackground()
        {
            Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            Shorewood.spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            hotShit.Begin();
            hotShit.CurrentTechnique.Passes[0].Begin();
            Shorewood.spriteBatch.End();
            hotShit.CurrentTechnique.Passes[0].End();
            hotShit.End();
        }

        public void DrawLogo()
        {
            Shorewood.spriteBatch.Begin();
            //Shorewood.spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            Shorewood.spriteBatch.Draw(logo, logoDrawPosition, Color.White);
            Shorewood.spriteBatch.End();

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);            
            DrawSolver();
            DrawBackground();
            DrawLogo();
            base.Draw(gameTime);
        }

        public void Show()
        {
            this.Visible = true;
            this.Enabled = true;
        }
        public void Hide()
        {
            this.Visible = false;
            this.Enabled = false;
        }

        public void StartAnimation(float startTime)
        {
            this.startTime = startTime;
            animationFinished = false;
            animationStarted = true;
            Show();
        }

        Floater SpawnFuzz(int y)
        {
            Vector2 triggerLocation = new Vector2(random.Next(1380), random.Next(y));
            return new Floater(triggerLocation, new Vector2(0, random.Next(5)), Color.White);
        }

        void DrawSolver()
        {
            Shorewood.spriteBatch.Begin();
            for (int i = 0; i < floaters.Length; i++)
            {
                Shorewood.spriteBatch.Draw(snowTexture, floaters[i].Position - new Vector2(100, 100), null, Color.White, 0f, Vector2.Zero, floaters[i].Scale, SpriteEffects.None, 1);
                if ((floaters[i].Position.Y > 920f) || (floaters[i].Position.X > 1400))
                {
                    floaters[i] = SpawnFuzz(0);
                }
            }
            Shorewood.spriteBatch.End();
        }
    }
}
