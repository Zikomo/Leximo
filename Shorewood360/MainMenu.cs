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
using IETGames.Shorewood.Localization;
using Particle3DSample;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Texture2D textBackgroundTexture = null;
        public Texture2D logo = null;
        public Rectangle viewPortRectangle = Rectangle.Empty;
        public float scale = 1;
        public PlayerIndex mainPlayer = PlayerIndex.One;
        public bool playerSelected = false;
        public bool canStart = false;
        SpriteBatch spriteBatch = null;
        public Vector2 logoPosition = new Vector2(100, 100);

        Vector2 playerSelectionTextPosition = new Vector2(100, 100);
        Vector2 startGameTextPosition = new Vector2(100, 100);
        public float logoSwapInDuration = 1000;
        public Vector2 logoStartPosition = new Vector2(1500, 0);
        Vector2 logoDrawPosition = Vector2.Zero;
        public DialogManager dialogManager;
        public Rectangle textBackgroundRectangle;
        float startTime = 0;
        public Vector2 explosionLocation;
        public bool isLogoInPlace = false;

        //public ParticleSystem explosionParticles;
        //public ParticleSystem explosionSmokeParticles;
        //Matrix viewMatrix;
        //Matrix projectionMatrix;
        //Matrix worldMatrix;

        public MainMenu(Game game, DialogManager dialogManager)
            : base(game)
        {
            this.dialogManager = dialogManager;
            
            // TODO: Construct any child components here
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
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //projectionMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 5, -5000, 5000);
            //worldMatrix = Matrix.Identity;

            spriteBatch = Shorewood.spriteBatch;// new SpriteBatch(GraphicsDevice);
            logo = Game.Content.Load<Texture2D>("screens\\logo");
            textBackgroundTexture = Game.Content.Load<Texture2D>("screens\\textBackground");
            logoPosition.X = Shorewood.titleSafeArea.Center.X - logo.Width / 2;
            logoPosition.Y = Shorewood.titleSafeArea.Top + 20;
            logoStartPosition += logoPosition * Vector2.UnitY;

            startGameTextPosition.Y = logoPosition.Y + logo.Height + 300;
            startGameTextPosition.X = Shorewood.titleSafeArea.Center.X - Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(Shorewood.localization[Shorewood.language][GameStrings.MenuPlayerSelect]).X / 2f;
            //startGameTextPosition.X = 600;
            playerSelectionTextPosition = startGameTextPosition;

            Vector2 size = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(Shorewood.localization[Shorewood.language][GameStrings.MenuPlayerSelect]);
            textBackgroundRectangle = new Rectangle((int)playerSelectionTextPosition.X, (int)playerSelectionTextPosition.Y, (int)size.X, (int)size.Y);
            textBackgroundRectangle.Inflate(5, 5);
            //explosionParticles = new ExplosionParticleSystem(this.Game, Shorewood.Content);            
            //explosionSmokeParticles = new ExplosionSmokeParticleSystem(this.Game, Shorewood.Content);

            //explosionParticles.DrawOrder = 400;
            //explosionSmokeParticles.DrawOrder = 100;
            //explosionParticles.Initialize();
            //explosionSmokeParticles.Initialize();
            //explosionParticles.SelfLoad();
            //explosionSmokeParticles.SelfLoad();
            //explosionParticles.SetCamera(viewMatrix, projectionMatrix);            
            //explosionSmokeParticles.SetCamera(viewMatrix, projectionMatrix);
            explosionLocation = logoPosition + (new Vector2(logo.Width, logo.Height) / 2);
            base.LoadContent();
        }

        public void Start(GameTime gameTime)
        {
            if (!Enabled || !Visible)
            {
                Enabled = true;
                Visible = true;
                Shorewood.constantBackground.Reset(gameTime);
                startTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
            if (playerSelected)
            {
#if XBOX                
                if (Gamer.SignedInGamers[Shorewood.mainPlayer] == null)
                {
                    if ((dialogManager.IsActive)&&(dialogManager.ActiveDialog.state == DialogState.Active))
                    {
                        dialogManager.CloseDialog(gameTime);
                    }
                    if (!Guide.IsVisible)
                    {
                        try
                        {
                            Guide.ShowSignIn(1, false);
                        }
                        catch (GuideAlreadyVisibleException)
                        {
                            //ignore
                        }
                    }
                    playerSelected = false;
                    return;

                }
#endif        
                if (!dialogManager.IsActive)
                {
                    if (!Shorewood.storage.StorageDevicePrompted)
                    {
                        Shorewood.storage.PromptForStorageDevice();
                    }
                    else
                    {
#if XBOX
                        if (Shorewood.storage.HighScoresLoaded && !Guide.IsVisible)
                        {
                            dialogManager.ShowDialog(DialogType.MainMenuDialog, gameTime);
                            
                        }
#else                        
                         dialogManager.ShowDialog(DialogType.MainMenuDialog, gameTime);
#endif
                    }

                }
            }
        }

        private void HandleStartGameplay(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:
                    dialogManager.CloseDialog(gameTime);
                    Shorewood.gameState = GameState.StartingNormalGameplay;
                    dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                    break;
            }
        }

        private void ShutDownPopUpResult(PopUpResultEventArgs result)
        {
            if (result.Result == DialogResult.A)
            {
                if (result.SelectedOption == 1)
                {
                    dialogManager.CloseDialog(result.GameTime);
                    Shorewood.gameState = GameState.ShutDown;
                    dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                }
            }
        }

        private void HandleExit(GameTime gameTime)
        {
            dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:

                    Shorewood.popUpManager.ShowDialog(PopUpType.Confirmation, gameTime, ShutDownPopUpResult);

                    break;
            }
        }

        private void HandleHighScores(GameTime gameTime)
        {
            
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:
                    dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                    dialogManager.GetDialog(DialogType.HighScores).parentDialogType = DialogType.MainMenuDialog;
                    dialogManager.ActiveDialog.TransitionTo(DialogType.HighScores, gameTime);
                    
                    //dialogManager.CloseDialog(gameTime);
                    //dialogManager.ShowDialog(DialogType.HighScores, gameTime);                    
                    break;

            }
        }

        private void HandleCredits(GameTime gameTime)
        {
            if (dialogManager.ActiveDialog.IsActive)
            {
                dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                dialogManager.CloseDialog(gameTime);
                Shorewood.creditsBackground.Visible = true;

            }
        }


        private void HandleResultA(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.SelectedOption)
            {
                case 0:
                    HandleStartGameplay(gameTime);
                    break;
                case 1:
                    HandleHighScores(gameTime);
                    break;
                case 2:
                    HandleCredits(gameTime);
                    break;
                case 3:
                    HandleProfileSwitch(gameTime);
                    break;
                case 4:
                    HandleExit(gameTime);
                    break;
                default:
                    dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                    break;
            }
        }

        private void HandleProfileSwitch(GameTime gameTime)
        {
            if (dialogManager.ActiveDialog.IsActive)
            {
                dialogManager.ActiveDialog.DialogResult = DialogResult.Nothing;
                dialogManager.CloseDialog(gameTime);
                ((Shorewood)Game).SwitchGamer();
            }
        } 

        private void UpdateMenu(GameTime gameTime)
        {
            if (dialogManager.IsActive && !Shorewood.popUpManager.IsActive)
            {
                if (dialogManager.ActiveDialogType == DialogType.MainMenuDialog)
                {
                    switch (dialogManager.ActiveDialog.DialogResult)
                    {
                        case DialogResult.A:
                            HandleResultA(gameTime);
                            break;
                    }
                }
            }
            else
            {
                
            }
        }

        private void CheckPlayer(GameTime gameTime)
        {
#if !XBOX   
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!playerSelected)
                {
                    Shorewood.gameState = GameState.Menu;
                    playerSelected = true;
                    Shorewood.storage.PromptForStorageDevice();
                }
            }
#else
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
            {
                Shorewood.mainPlayer = PlayerIndex.One;
                playerSelected = true;  
                
            }
            else if (GamePad.GetState(PlayerIndex.Two).Buttons.Start == ButtonState.Pressed)
            {
                Shorewood.mainPlayer = PlayerIndex.Two;
                playerSelected = true;
            }

            else if (GamePad.GetState(PlayerIndex.Three).Buttons.Start == ButtonState.Pressed)
            {
                Shorewood.mainPlayer = PlayerIndex.Three;
                playerSelected = true;
            }

            else if (GamePad.GetState(PlayerIndex.Four).Buttons.Start == ButtonState.Pressed)
            {
                Shorewood.mainPlayer = PlayerIndex.Four;
                playerSelected = true;
            }
            Shorewood.gameState = GameState.Menu;
           
#endif

        }

        private void UpdatePlayerSelection(GameTime gameTime)
        {
            CheckPlayer(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        //public void TriggerExplosion(GameTime gameTime, Vector2 explosionLocation)
        //{
        //    for (int i = 0; i < 20; i++)
        //        explosionParticles.AddParticle(Unproject(explosionLocation), new Vector3(4,31,0));

        //    for (int i = 0; i < 50; i++)
        //        explosionSmokeParticles.AddParticle(Unproject(explosionLocation), new Vector3(4, 31, 0));
        //}

        //private Vector3 Unproject(Vector2 location)
        //{
        //    return GraphicsDevice.Viewport.Unproject(new Vector3(location, 0), projectionMatrix, viewMatrix, worldMatrix);
        //}

        public override void Update(GameTime gameTime)
        {
            //explosionParticles.ChangeTexture(Shorewood.Is1337Compliant);
            //explosionSmokeParticles.ChangeTexture(Shorewood.Is1337Compliant);
            if (playerSelected == false)
            {
                UpdatePlayerSelection(gameTime);
            }
            UpdateMenu(gameTime);
            if ((logoDrawPosition == logoPosition)&&(!isLogoInPlace))
            {
                isLogoInPlace = true;
                //explosionParticles.Update(gameTime);
                //explosionSmokeParticles.Update(gameTime);
                //Shorewood.particleSystem.TriggerExplosion(gameTime, explosionLocation);
            }
            else
            {
                UpdateLogo(gameTime);
            }
            base.Update(gameTime);
        }

        private void UpdateLogo(GameTime gameTime)
        {
            logoDrawPosition = Vector2.SmoothStep(logoStartPosition, logoPosition, ((float)gameTime.TotalGameTime.TotalMilliseconds - startTime) / logoSwapInDuration);
        }

        public void DrawText()
        {
            if (!playerSelected)
            {
                if (logoDrawPosition == logoPosition)
                {
                    spriteBatch.Draw(textBackgroundTexture, textBackgroundRectangle, Color.White);
                    spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont], Shorewood.localization[Shorewood.language][GameStrings.MenuPlayerSelect], playerSelectionTextPosition, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                }
            }
        }

        //public void DrawBackground()
        //{
        //    spriteBatch.Draw(backgroundTexture, viewPortRectangle, null, Color.White);                                        
        //}

        public void DrawLogo()
        {            
            spriteBatch.Draw(logo, logoDrawPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }

        public override void Draw(GameTime gameTime)
        {
            //explosionParticles.Draw(gameTime);
            //explosionSmokeParticles.Draw(gameTime);
            spriteBatch.Begin();
            DrawLogo();
            DrawText();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}