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
//using ProjectMercury;
//using BloomPostprocess;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;
//using ProjectMercury.Renderers;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>

    public struct ColorNode
    {
        public float duration;
        public float startTime;

        public Color endColor;
        public ColorNode(float duration, Color endColor)
        {
            this.startTime = 0;
            this.duration = duration;
            this.endColor = endColor;
 
        }
    }

    public struct WandererNode
    {
        public Vector2 direction;
        public Vector2 position;
        public float scale;
        public Vector2 origin;
        public float rotation;
        public Color color;
        public Texture2D texture;
        public WandererNode(Vector2 direction, Vector2 position, Texture2D texture)
        {
            this.direction = direction;
            this.position = position;
            this.scale = 0;
            this.origin = Vector2.Zero;
            this.rotation = 0;
            this.color = Color.White;
            this.texture = texture;
        }
    }
    
    public class ConstantBackground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D background;
        Texture2D backGroundColorTexture;
        Texture2D mask;
        Effect hotShit;
        
        //ParticleEffect particles;
        
       // SpriteBatchRenderer renderer;
        List<Body> particleSprites = new List<Body>(10);
        List<WandererNode> wanderers = new List<WandererNode>(5);
        Texture2D[] wanderTexture = new Texture2D[3];
        //Vector3 backgroundColorVector;// = Color.Black;
        Color backgroundColor;
        Color previousBackgroundColor;

        public float maxParticleVelocity = 20f;
        public float minParticleVelocity = 0.1f;
        private float particleVelocity = 0.0f;
        private float previousParticleVelocity = 0.00f;
        private float nextParticleVelocity;
        private float accelerationDuration = 6000;
        private float accelrationStartTime = 0;
        private bool velocityChanged = false;
        private TimeSpan resetTimer = new TimeSpan();
        private int resetFluidsTime = 1;
        
        public List<ColorNode> colorLoop = new List<ColorNode>(20);
        int colorIndex = 0;
        //BloomComponent blur;
        bool firstPass = true;
        public ConstantBackground(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            //backgroundColorVector = new Vector3();
            backgroundColor = Color.Black;
            previousBackgroundColor = Color.White;
            colorLoop.Add(new ColorNode(30000, Color.White));                        
            colorLoop.Add(new ColorNode(30000, Color.Purple));            
            colorLoop.Add(new ColorNode(30000, Color.Blue));            
            colorLoop.Add(new ColorNode(30000, Color.Pink));
            colorLoop.Add(new ColorNode(30000, Color.Red));
            colorLoop.Add(new ColorNode(30000, Color.Orange));
            //blur = new BloomComponent(game);
            particleVelocity = minParticleVelocity;
            Enabled = false;
            Visible = false;
            //wanderers.Add(new WandererNode(new Vector2(1,1), new Vector2(100,100)));

        }

        public bool Is1337
        {
            set
            {
                hotShit.Parameters["use1337"].SetValue(value);
            }
            get
            {
                return hotShit.Parameters["use1337"].GetValueBoolean();
            }
        }
        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            base.OnVisibleChanged(sender, args);
        }
        protected override void LoadContent()
        {
            background = Shorewood.Content.Load<Texture2D>("screens\\baseBackgroundOverlay");

            //particles = Shorewood.Content.Load<ParticleEffect>("ParticleEffects\\BackgroundParticles");
            wanderTexture[0] = Shorewood.Content.Load<Texture2D>("Sprites\\baseRing0");
            wanderTexture[1] = Shorewood.Content.Load<Texture2D>("Sprites\\baseRing1");
            wanderTexture[2] = Shorewood.Content.Load<Texture2D>("Sprites\\baseRing2");
            mask = Shorewood.Content.Load<Texture2D>("screens\\mask");
            
            hotShit = Shorewood.Content.Load<Effect>("Effects\\HotShit");
            hotShit.Parameters["mask"].SetValue(mask);
            //particles.LoadContent(Shorewood.Content);
            //renderer = new SpriteBatchRenderer();
            //particles.Initialise();
            
            //blur.SelfLoad();
            //blur.Settings = BloomSettings.PresetSettings[(int)BloomType.Blurry];
            Random random = new Random();

            for (int i = 0; i < 64; i++)
            {
                Vector2 direction = new Vector2(random.Next(-5,5), random.Next(-5,5));
                WandererNode node = new WandererNode(direction,
                    new Vector2(random.Next(0, Shorewood.viewPortRectangle.Width), random.Next(0, Shorewood.viewPortRectangle.Height)),
                    wanderTexture[random.Next(3)]
                    );
                   //new Vector2((1f-(float)i/64f)*1280f,360f));
                node.origin = new Vector2(wanderTexture[0].Width / 2, wanderTexture[0].Height / 2);
                node.color = new Color(Vector3.Lerp(Color.Orange.ToVector3(), Color.Green.ToVector3(), (float)i / 256f));
                node.color.A = 128;

                wanderers.Add(node);
            }
            previousReducedFrequencies = new float[wanderers.Count];
            backGroundColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] white = { Color.White };
            backGroundColorTexture.SetData<Color>(white);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            //blur.Initialize();
            base.Initialize();
        }

        public float ParticleVelocity
        {
            get
            {
                return particleVelocity;
            }
            set
            {
                previousParticleVelocity = particleVelocity;
                nextParticleVelocity = value*10;
                velocityChanged = previousParticleVelocity != nextParticleVelocity;
                //particleVelocity = value*10;
            }
        }
        float[] previousReducedFrequencies;
        public void UpdateWanderers(GameTime gameTime)
        {
            VisualizationData viz = Shorewood.soundTrack.GetVisualizationData();
            int frequenciesPerWanderer = viz.Frequencies.Count/wanderers.Count;
            float[] reducedFrequencies = new float[wanderers.Count];
            bool trigger = false;
            for (int i = 0; i < wanderers.Count; i++)
            {
                int indexStart = i * frequenciesPerWanderer;
                float averageFrequency = 0;
                for (int j = indexStart; j < indexStart + frequenciesPerWanderer; j++)
                {
                    averageFrequency += viz.Frequencies[j];
                }
                reducedFrequencies[i] = averageFrequency / frequenciesPerWanderer;
                float percentDifference = Math.Abs(reducedFrequencies[i] - previousReducedFrequencies[i]) / previousReducedFrequencies[i];
                if ((percentDifference > 2.5f)||(!Shorewood.soundTrack.GameHasControl))
                {
                    trigger = true;
                }
            }
            UpdateMaster();
           
            for (int i = 1; i < wanderers.Count; i++)
            {
                WandererNode node = wanderers[i];

                if ((node.position.X < 0) || (node.position.X > Shorewood.viewPortRectangle.Width))
                {
                    node.direction.X = 0 - node.direction.X;
                }
                if ((node.position.Y < 0) || (node.position.Y > Shorewood.viewPortRectangle.Height))
                {
                    node.direction.Y = 0 - node.direction.Y;
                }
                
                node.position += node.direction * particleVelocity;
                if (Shorewood.soundTrack.GameHasControl)
                {
                    node.scale = (float)(Math.Exp(reducedFrequencies[i]) - 1);
                }
                else
                {
                    node.scale = 1;
                }
                wanderers[i] = node;               
            }
            if (trigger && Shorewood.menu.isLogoInPlace)
            {
                if ((Shorewood.gameState == GameState.PlayerSelection)||(Shorewood.gameState == GameState.Menu)||(Shorewood.gameState == GameState.Credits))
                {
                    Shorewood.particleSystem.TriggerExplosion(gameTime, Shorewood.menu.explosionLocation);
                }
            }
            previousReducedFrequencies = reducedFrequencies;            
        }

        private void UpdateMaster()
        {
            WandererNode node = wanderers[0];
            
            if ((node.position.X < 0) || (node.position.X > Shorewood.viewPortRectangle.Width))
            {

                node.direction.X = 0 - node.direction.X;
            }
            if ((node.position.Y < 0) || (node.position.Y > Shorewood.viewPortRectangle.Height))
            {
                node.direction.Y = 0 - node.direction.Y;
            }
            node.position += (node.direction)*particleVelocity ;
            node.scale = 5;
            wanderers[0] = node;
            
        }

        public void UpdateColor(GameTime gameTime)
        {
            ColorNode currentNode = colorLoop[colorIndex % colorLoop.Count];
            if (firstPass)
            {
                currentNode.startTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                firstPass = false;
            }
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds - currentNode.startTime;
            if (time < currentNode.duration)
            {
                backgroundColor = new Color(Vector3.SmoothStep(previousBackgroundColor.ToVector3(), currentNode.endColor.ToVector3(), time / currentNode.duration));

            }
            else
            {
                NextColor(gameTime);
            }
        }

        private void UpdateVelocity(GameTime gameTime)
        {
            float time = ((float)gameTime.TotalGameTime.TotalMilliseconds - accelrationStartTime) / accelerationDuration;
            if (velocityChanged)
            {                
                accelrationStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                velocityChanged = false;
                if (time < 1)
                {
                    particleVelocity = nextParticleVelocity;
                    return;
                }
            }
            
            if (time < 1)
            {
                particleVelocity = MathHelper.Lerp(previousParticleVelocity, nextParticleVelocity, time);
                
            }
        }

        private void NextColor(GameTime gameTime)
        {
            previousBackgroundColor = backgroundColor;
            colorIndex++;
            ColorNode nextNode = colorLoop[colorIndex % colorLoop.Count];
            nextNode.startTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            colorLoop[colorIndex % colorLoop.Count] = nextNode;

        }

        public void Reset(GameTime gameTime)
        {
            ParticleVelocity = minParticleVelocity;
            NextColor(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //particles.Trigger(new Vector2(100,100));
            resetTimer += gameTime.ElapsedGameTime;
            if (resetTimer.Seconds < resetFluidsTime)
            {
                //Shorewood.fluidSolver.reset();
                //Shorewood.fluidSolver.Trigger((int)wanderers[0].position.X, (int)wanderers[0].position.Y, 1); 
                resetTimer = new TimeSpan();
            }
            UpdateVelocity(gameTime);
            UpdateWanderers(gameTime);
            
            //blur.Update(gameTime);
            UpdateColor(gameTime);
            base.Update(gameTime);
        }

        public void DrawBackground(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            Shorewood.spriteBatch.Draw(background, Vector2.Zero, Color.White);
            hotShit.Begin();
            hotShit.CurrentTechnique.Passes[0].Begin();
            Shorewood.spriteBatch.End();
            hotShit.CurrentTechnique.Passes[0].End();
            hotShit.End();         
        }

        public void DrawBackgroundColor(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(backGroundColorTexture, Shorewood.viewPortRectangle, backgroundColor);
            Shorewood.spriteBatch.End();

            //GraphicsDevice.Clear(backgroundColor);
        }

        public void DrawParticles(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin(SpriteBlendMode.Additive);
            for (int i = 1; i < wanderers.Count; i++)
            {
                Shorewood.spriteBatch.Draw(wanderers[i].texture, wanderers[i].position, null, wanderers[i].color, wanderers[i].rotation, wanderers[i].origin, wanderers[i].scale, SpriteEffects.None, 1);
            }
            //renderer.RenderEffect(particles, Shorewood.spriteBatch);
            Shorewood.spriteBatch.End();
        }

        public void DrawBlur(GameTime gameTime)
        {
            if (!Shorewood.bloom.Visible)
            {
                //blur.Draw(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawBackgroundColor(gameTime);
            DrawParticles(gameTime);
            DrawBackground(gameTime);
            //DrawBlur(gameTime);           
                        
            base.Draw(gameTime);
        }
    }
}