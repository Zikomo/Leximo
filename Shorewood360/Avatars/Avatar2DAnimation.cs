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


namespace IETGames.Shorewood.Avatars
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    
    
    public class Avatar2DAnimation : Microsoft.Xna.Framework.DrawableGameComponent
    {
        AvatarDescription avatarDescription;
        AvatarAnimation avatarAnimation;
        AvatarRenderer avatarRenderer;

        // Render Target and Texture  
        RenderTarget2D avatarRenderTarget;
        DepthStencilBuffer depthStencilBuffer;
        //List<Texture2D> AvatarAnamation = new List<Texture2D>(60);

        int spriteWidth = 256;
        int spriteHeight = 256; 


        public Avatar2DAnimation(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Visible = true;
            AvatarAnimation = new List<Texture2D>[3];
            for (int i = 0; i < AvatarAnimation.Length; i++)
            {
                AvatarAnimation[i] = new List<Texture2D>(60);
            }

        }
        public List<Texture2D>[] AvatarAnimation
        {
            get;
            private set;
        }

        void TryLoadAvatar()
        {
            if (!Shorewood.presetAvatarAnimations.Keys.Contains<AvatarAnimationPreset>(AvatarAnimationPreset.Celebrate))
            {
                Shorewood.presetAvatarAnimations.Add(AvatarAnimationPreset.Celebrate, new AvatarAnimation(AvatarAnimationPreset.Celebrate));
                Shorewood.presetAvatarAnimations.Add(AvatarAnimationPreset.Clap, new AvatarAnimation(AvatarAnimationPreset.Clap));
            }
            
            avatarDescription = AvatarDescription.CreateRandom();

            // Load avatar animation information  
            avatarAnimation = Shorewood.presetAvatarAnimations[AvatarAnimationPreset.Celebrate];
            // Create new avatar Renderer  
            avatarAnimation.CurrentPosition = TimeSpan.Zero;
            avatarRenderer = new AvatarRenderer(avatarDescription, false);

        }
        protected override void LoadContent()
        {
            
            TryLoadAvatar();
            // Create Render Target for the avatar sprite  
            avatarRenderTarget = new RenderTarget2D(GraphicsDevice, spriteWidth, spriteHeight, 1, SurfaceFormat.Color, MultiSampleType.FourSamples, 0);
            depthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, spriteWidth, spriteHeight, GraphicsDevice.DepthStencilBuffer.Format, MultiSampleType.FourSamples, 0);
            base.LoadContent();
        }

        private Texture2D TransferTexture(Texture2D texture)
        {
            Texture2D rtnTexture = new Texture2D(GraphicsDevice, texture.Width, texture.Height);            
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
            rtnTexture.SetData<Color>(pixels);
            return rtnTexture;
        }

        int j = 0;
        
        private void UpdateAvatarSprite(GameTime gameTime)
        {
            if (avatarRenderer == null)
            {
                TryLoadAvatar();
            }
            if (!avatarRenderer.IsDisposed && avatarRenderer.IsLoaded)
            {
                if (j < 3)
                {
                    if (avatarAnimation.CurrentPosition < avatarAnimation.Length)
                    {
                        DepthStencilBuffer oldStencil = GraphicsDevice.DepthStencilBuffer;
                        // Set Render Target and Clear  
                        GraphicsDevice.SetRenderTarget(0, avatarRenderTarget);
                        GraphicsDevice.DepthStencilBuffer = depthStencilBuffer;
                        GraphicsDevice.Clear(Color.TransparentBlack);

                        // Update Animation  
                        avatarAnimation.Update(new TimeSpan(0, 0, 0, 0, 100), false);

                        // Set Avatar Renderer Properties and draw  
                        avatarRenderer.World = Matrix.Identity;
                        avatarRenderer.View = Matrix.CreateLookAt(new Vector3(0, avatarDescription.Height / 2.0f, -2.5f), new Vector3(0, avatarDescription.Height / 2.0f, 0), Vector3.Up);
                        avatarRenderer.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)spriteWidth / (float)spriteHeight, 0.1f, 1000);
                        avatarRenderer.Draw(avatarAnimation.BoneTransforms, avatarAnimation.Expression);

                        // Set Render Target back to the back buffer  
                        GraphicsDevice.DepthStencilBuffer = oldStencil;
                        GraphicsDevice.SetRenderTarget(0, null);

                        // Resolve render target to the texture  

                        AvatarAnimation[j].Add(TransferTexture(avatarRenderTarget.GetTexture()));
                    }
                    else
                    {
                        if (j < 3)
                        {
                            avatarRenderer.Dispose();
                            TryLoadAvatar();
                            j++;
                        }
                    }
                }
                else
                {
                    avatarRenderer.Dispose();
                    avatarRenderTarget.Dispose();
                    Visible = false;
                    Shorewood.freeSpellMode.LoadAnimation();
                }

            }
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateAvatarSprite(gameTime);
            base.Draw(gameTime);
        }
    }
}