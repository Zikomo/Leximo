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
using Algorithms;
using System.Reflection;
using System.Text;
using BloomPostprocess;
using IETGames.Shorewood.Localization;
using FarseerGames.FarseerPhysics;
using IETGames.Shorewood.Diagnostics;
using IETGames.Shorewood.Threads;
using IETGames.Shorewood.Utilities;
using Fluids;
using IETGames.Shorewood.Storage;
using IETGames.Shorewood.Input;
using KiloWatt.Runtime.Support;
using IETGames.Shorewood.Avatars;

namespace IETGames.Shorewood
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public enum Language
    {
        USEnglish,
        Spanish
    }

    public enum GameState
    {
        RunOnce,
        SplashScreen,
        Loading,
        PlayerSelection,
        Menu,
        StartingNormalGameplay,
        PlayingNormalGameplay,
        ResettingNormalGameplay,
        Paused,
        ShowingHighScore,
        GameOver,
        ShutDown,
        HowToPlay,
        StorageActive,
        FailedPurchase,
        Credits
    }

    public enum FontTypes
    {
        MenuFont,
        DebugFont,
        FloatingPointsFont,
        GoalFont,
        MenuButtonFont,
        ScoreFont,
        Font1337
    }

    public enum Difficulty:int
    { 
        FreeSpell = 0, 
        Easy = 1, 
        Medium = 2,         
        Hard = 3, 
        _1337 = 4 
    }

    public class Shorewood : Microsoft.Xna.Framework.Game
    {
        public static Dictionary<AvatarAnimationPreset, AvatarAnimation> presetAvatarAnimations = new Dictionary<AvatarAnimationPreset, AvatarAnimation>(4);
        public static PlayerIndex mainPlayer = PlayerIndex.One;        
        public static SuffixTree wordDatabase;
        public new static ContentManager Content;
        public static Dictionary<FontTypes, SpriteFont> fonts = new Dictionary<FontTypes, SpriteFont>();
        public static Dictionary<Language, Dictionary<GameStrings, StringBuilder>> localization = new Dictionary<Language, Dictionary<GameStrings, StringBuilder>>();
        public static Language language = Language.USEnglish;
        public static int idCounter = 0;
        public static Rectangle titleSafeArea = Rectangle.Empty;
        public static DialogManager dialogManager;
        
        public static GameState gameState = GameState.RunOnce;
        //public static PointsParticles pointsParticles;
        public static GraphicsDeviceManager graphics;
        public static float scale;
        public static BloomComponent bloom;
        public static Rectangle viewPortRectangle = Rectangle.Empty;
        public static SpriteBatch spriteBatch;
        public static PhysicsSimulator physicsEngine;
        public static ScoreBox scoreBox;
        public static RecentWordsRenderer recentWords;
        public static GridScreenRenderer normalGameplayRenderer;
        public static ConstantBackground constantBackground;
        public static SoundTrack soundTrack;
        public static NormalGameplay normalModeGamePlay;
        public static Dictionary<char, Letter> alphabet;
        public static ILetterSet letterSet;
        public static FoundWordAnimationRenderer foundWordAnimator;
        public static TimerComponent gameplayTimer;
        public static GoalRenderer goalRenderer;
        public static FluidSolverComponent fluidSolver;
        public static TimerRenderer timerRenderer;
        public static MainMenu menu;
        public static NormalGameplayLevelManager levelManager;
        //public static _1337Shader shader1337;
        public static UniversalParticleSystem particleSystem;
        public static Difficulty Difficulty = Difficulty.FreeSpell;
        public static StorageHandler storage;
        public static InputHandler inputHandler;
        public static GameState previousGameState;
        public static StorageScreen storageScreen;
        public static ThreadPoolComponent threadPool;
        public static PopUpManager popUpManager;
        public static FreeSpellMode freeSpellMode;
        public static Avatar2DAnimation avatar2DAnimation;
        public static SoundEffect cheer;
        public static SoundEffect pew;
        public static SoundEffect boomOne;
        public static SoundEffect boomTwo;
        public static SoundEffect boomThree;
        public static SoundEffect boomFour;
        public static SoundEffect sparkles;        
        public static SoundEffect whooshout;
        public static SoundEffect whooshin;
        public static SoundEffect clink;
        public static SoundEffect tick;
        public static SoundEffect pop;
        public static SoundEffect scroll;
        public static uint[] peoplePixels;
        public static CreditsBackground creditsBackground;
        public static CreditsForeground creditsForeground;
        NinthPlanetGames.SplashScreen ninthPlanetGames;

        private static bool is1337Compliant = false;
        public static bool Is1337Compliant
        {
            get
            {
                return is1337Compliant;
            }
            set
            {
                is1337Compliant = value;
                constantBackground.Is1337 = value;
                recentWords.Changed = true;
                particleSystem.explosionParticles.ChangeTexture(is1337Compliant);
                particleSystem.explosionSmokeParticles.ChangeTexture(is1337Compliant);
            }
        }

        public static bool IsStorageActive
        {
            get
            {
                return gameState == GameState.StorageActive;
            }
            set
            {
                if (value)
                {
                    if (gameState != GameState.StorageActive)
                    {
                        previousGameState = gameState;
                    }

                    gameState = GameState.StorageActive;
                    storageScreen.Enabled = true;
                    storageScreen.Visible = true;
                }
                else
                {
                    gameState = previousGameState;
                    storageScreen.Enabled = false;
                    storageScreen.Visible = false;
                }
            }
        }
        

        float fps;
        
        DisplayMode displayMode;
        Vector2 loadingPosition = Vector2.Zero;       
#if XBOX
        public static GamerServicesComponent services;
#endif
        bool isLoaded = false;
        private static TimeSpan timeSinceLastSparkle = TimeSpan.Zero;
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static RenderTarget2D CloneRenderTarget(GraphicsDevice device,
            int numberLevels)
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                numberLevels,
                device.DisplayMode.Format,
                device.PresentationParameters.MultiSampleType,
                device.PresentationParameters.MultiSampleQuality
            );
        }        

 
        private void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e) 
        {      
             e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;          
        }

        public static RenderTarget2D CreateRenderTarget(GraphicsDevice device,
               int numberLevels, SurfaceFormat surface)
        {
            MultiSampleType type =
                device.PresentationParameters.MultiSampleType;

            // If the card can't use the surface format
            if (!GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(
                DeviceType.Hardware,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format,
                TextureUsage.None,
                QueryUsages.None,
                ResourceType.RenderTarget,
                surface))
            {
                // Fall back to current display format
                surface = device.DisplayMode.Format;
            }
            // Or it can't accept that surface format 
            // with the current AA settings
            else if (!GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(
                DeviceType.Hardware, surface,
                device.PresentationParameters.IsFullScreen, type))
            {
                // Fall back to no antialiasing
                type = MultiSampleType.None;
            }

            int width, height;

            // See if we can use our buffer size as our texture
            CheckTextureSize(device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                out width, out height);

            // Create our render target
            return new RenderTarget2D(device,
                width, height, numberLevels, surface,
                type, 0);

        }

        public static bool CheckTextureSize(int width, int height,
            out int newwidth, out int newheight)
        {
            bool retval = false;

            GraphicsDeviceCapabilities Caps;
            Caps = GraphicsAdapter.DefaultAdapter.GetCapabilities(
                DeviceType.Hardware);

            if (Caps.TextureCapabilities.RequiresPower2)
            {
                retval = true;  // Return true to indicate the numbers changed

                // Find the nearest base two log of the current width, 
                // and go up to the next integer                
                double exp = Math.Ceiling(Math.Log(width) / Math.Log(2));
                // and use that as the exponent of the new width
                width = (int)Math.Pow(2, exp);
                // Repeat the process for height
                exp = Math.Ceiling(Math.Log(height) / Math.Log(2));
                height = (int)Math.Pow(2, exp);
            }
            if (Caps.TextureCapabilities.RequiresSquareOnly)
            {
                retval = true;  // Return true to indicate numbers changed
                width = Math.Max(width, height);
                height = width;
            }

            newwidth = Math.Min(Caps.MaxTextureWidth, width);
            newheight = Math.Min(Caps.MaxTextureHeight, height);
            return retval;
        }

        public static bool IsTrial
        {
            get
            {
#if XBOX
                return Guide.IsTrialMode;
                //return true;
#else
                return true;
#endif
            }
        }

        public static DepthStencilBuffer CreateDepthStencil(
    RenderTarget2D target)
        {
            return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
                target.Height, target.GraphicsDevice.DepthStencilBuffer.Format,
                target.MultiSampleType, target.MultiSampleQuality);
        }
        public static DepthStencilBuffer CreateDepthStencil(
            RenderTarget2D target, DepthFormat depth)
        {
            if (GraphicsAdapter.DefaultAdapter.CheckDepthStencilMatch(
                DeviceType.Hardware,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format,
                target.Format,
                depth))
            {
                return new DepthStencilBuffer(target.GraphicsDevice,
                    target.Width, target.Height, depth,
                    target.MultiSampleType, target.MultiSampleQuality);
            }
            else
                return CreateDepthStencil(target);
        }

        public static Matrix UpdateTranslationMatrix(Vector2 position)
        {
            return Matrix.CreateTranslation(new Vector3(position, 0));
        }


        public Shorewood()
        {
            graphics = new GraphicsDeviceManager(this);
            threadPool = new ThreadPoolComponent(this);
            inputHandler = new InputHandler(this);
            inputHandler.AddEvent(Buttons.Back, new EventHandler<ButtonFireEventArgs>(OnPause));
            inputHandler.AddEvent(Buttons.Start, new EventHandler<ButtonFireEventArgs>(OnPause));
            storageScreen = new StorageScreen(this);
            
            foundWordAnimator = new FoundWordAnimationRenderer(this);
            constantBackground = new ConstantBackground(this);
            fluidSolver = new FluidSolverComponent(this);
            scoreBox = new ScoreBox(this);
            particleSystem = new UniversalParticleSystem(this);
            constantBackground.Enabled = false;
            constantBackground.Visible = false;
            
            
            foundWordAnimator.Enabled = false;
            foundWordAnimator.Visible = false;
            graphics.PreferredBackBufferWidth = PlatformDisplaySettings.width;
            graphics.PreferredBackBufferHeight = PlatformDisplaySettings.height;
            fluidSolver.Differences = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            IsFixedTimeStep = false;
            
            
            Shorewood.Content = base.Content;
            Content.RootDirectory = "Content";
            scale = graphics.PreferredBackBufferHeight / 1000.0f;
            soundTrack = new SoundTrack(this);
            ninthPlanetGames = new NinthPlanetGames.SplashScreen(this);       
            
            
            ninthPlanetGames.Visible = true;
            ninthPlanetGames.Enabled = true;

            bloom = new BloomComponent(this);
            
            loadingPosition = (new Vector2(PlatformDisplaySettings.width / 2, PlatformDisplaySettings.height / 2));
            graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;
            ShorewoodPool.InitalizePools();
            physicsEngine = new PhysicsSimulator(new Vector2(0, 100));


#if XBOX
            services = new GamerServicesComponent(this);
            Components.Add(services);               
#endif

            Components.Add(threadPool);
            storage = new StorageHandler();
            Components.Add(inputHandler);
            Components.Add(storage.sharedSaveDevice);
            Components.Add(soundTrack);
            Components.Add(fluidSolver);
            Components.Add(ninthPlanetGames);
            avatar2DAnimation = new Avatar2DAnimation(this);
            Components.Add(avatar2DAnimation);
            //Components.Add(ietGames);
            
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            Shorewood.fonts.Add(FontTypes.DebugFont, Content.Load<SpriteFont>("Fonts\\Courier New"));
            Shorewood.fonts.Add(FontTypes.MenuFont, Content.Load<SpriteFont>("Fonts\\Arial"));
            Shorewood.fonts.Add(FontTypes.FloatingPointsFont, Content.Load<SpriteFont>("Fonts\\Nonstop.font"));
            Shorewood.fonts.Add(FontTypes.GoalFont, Content.Load<SpriteFont>("Fonts\\Arial.Smaller"));
            Shorewood.fonts.Add(FontTypes.MenuButtonFont, Content.Load<SpriteFont>("Fonts\\Arial.Large.Glyphs"));
            Shorewood.fonts.Add(FontTypes.ScoreFont, Content.Load<SpriteFont>("Fonts\\Arial.Smaller"));
            Shorewood.fonts.Add(FontTypes.Font1337, Content.Load<SpriteFont>("Fonts\\elite"));
            displayMode = GraphicsDevice.DisplayMode;
            
            Shorewood.localization.Add(UsEnglishLocalization.Language, UsEnglishLocalization.StringTable);
            ConstructAlphabet();

            titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
#if !XBOX
            titleSafeArea = new Rectangle(128, 72, 1024, 576);
#endif
            dialogManager = new DialogManager(this);
            popUpManager = new PopUpManager(this);
            menu = new MainMenu(this, dialogManager);
            menu.Enabled = false;
            menu.Visible = false;
            

            scale = titleSafeArea.Height / 1000.0f;
            base.Initialize();
        }

        public void SetupLocalization()
        {
 
        }

        private void ConstructAlphabet()
        {
            alphabet = UsEnglishLocalization.Alphabet;
        }

        public static void PlaySparkles()
        {
            if (timeSinceLastSparkle > Shorewood.sparkles.Duration)
            {
                Shorewood.sparkles.Play(0.35f,0,0);
                timeSinceLastSparkle = TimeSpan.Zero;
                boomCount = Random.Next();
            }
        }
        static int boomCount = 0;
        public static void PlayBoom(Vector3 argPosition)
        {
            Vector2 position = particleSystem.Project(argPosition);
            position.X = (position.X - 640f) / 640f;
            int boom = Random.Next(0, 4);
            //int boom = (++boomCount) % 4;
            switch (boom)
            {
                case 0:
                    Shorewood.boomOne.Play(1,0, position.X );
                    break;
                case 1:
                    Shorewood.boomTwo.Play(1, 0, position.X );
                    break;

                case 2:
                    Shorewood.boomThree.Play(1, 0, position.X );
                    break;

                case 3:
                    Shorewood.boomFour.Play(1, 0, position.X );
                    break;

            }
        }


        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
#if XBOX
            Guide.NotificationPosition = NotificationPosition.BottomRight;
#endif
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            viewPortRectangle = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //backgroundTexture = Content.Load<Texture2D>("screens\\basebackground");
            
            SetupLocalization();
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            menu.scale = scale;
            menu.viewPortRectangle = viewPortRectangle;
            storage.HighScores.Load();
            inputHandler.ControllerDisconnect += new EventHandler<ControllerDisconnectedEventArgs>(ControllerDisconnected);
            peoplePixels = new uint[64 * 64];
            Texture2D people = Content.Load<Texture2D>("Sprites\\tag");
            cheer = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\22952__acclivity__Cheer");
            pew = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\pew");
            boomOne = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\boomOne");
            boomTwo = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\boomTwo");
            boomThree = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\boomThree");
            boomFour = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\boomFour");
            sparkles = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\sparkles");
            whooshout = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\wooshout");
            whooshin = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\wooshin");
            tick = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\tick");
            clink = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\clink");
            pop = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\pop");
            scroll = Shorewood.Content.Load<SoundEffect>(@"Audio\Sfx\scroll");
            people.GetData<uint>(peoplePixels);
            //inputHandler.ControllerDisconnect += new EventHandler<ControllerDisconnectedEventArgs>(
            base.LoadContent();
        }

        void ControllerDisconnected(object sender, ControllerDisconnectedEventArgs e)
        {
            if (e.PlayerIndex == Shorewood.mainPlayer)
            {
                previousGameState = gameState;
                if (gameState == GameState.PlayingNormalGameplay)
                {
                    gameState = GameState.Paused;
                }
                popUpManager.ShowDialog(PopUpType.ControllerDisconnected, new GameTime(), ControllerDisconnectResolved);
            }
        }

        void ControllerDisconnectResolved(PopUpResultEventArgs e)
        {
            if (previousGameState != GameState.PlayingNormalGameplay)
            {
                gameState = previousGameState;
            }
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            if (e.Gamer.PlayerIndex == mainPlayer)
            {
                
            }
        }

        public void SwitchGamer()
        {
            HideGameplay();
            normalModeGamePlay.Reset(new GameTime());
            menu.playerSelected = false;
            if (!dialogManager.IsActive)
            {
                menu.playerSelected = false;
            }
            else
            {

                dialogManager.CloseDialog(new GameTime());
            }
            gameState = GameState.PlayerSelection;

        }

        void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
        {
            if (e.Gamer.PlayerIndex == mainPlayer)
            {
                SwitchGamer();
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            PredictiveTextAnalyzer.Stop();       
        }

        public static void OnPurchase(PopUpResultEventArgs a)
        {
            if (a.Result == DialogResult.A)
            {
                if (a.SelectedOption == 1)
                {
                    if (Gamer.SignedInGamers[Shorewood.mainPlayer].Privileges.AllowPurchaseContent)
                    {
                        try
                        {
                            Shorewood.cheer.Play();
                            Guide.ShowMarketplace(Shorewood.mainPlayer);
                        }
                        catch (GuideAlreadyVisibleException)
                        {
                        }
                    }
                    else
                    {
                        //TransitionTo(PopUpType.CannotPurchase, a.GameTime);
                        previousGameState = gameState;
                        gameState = GameState.FailedPurchase;
                    }
                }
            }
        }

        private void UpdateRunOnce(GameTime gameTime)
        {
            ninthPlanetGames.StartAnimation(gameTime.TotalGameTime.Milliseconds);
            gameState = GameState.Loading;
        }

        private void UpdateSplashScreen(GameTime gameTime)
        {
            if (ninthPlanetGames.animationFinished)
            {
                
                gameState = GameState.PlayerSelection;
            }
            else if (!ninthPlanetGames.animationStarted)
            {
                ninthPlanetGames.StartAnimation(gameTime.TotalGameTime.Milliseconds);
            }
        }

        private void UpdateLoading(GameTime gameTime)
        {
            if (!isLoaded)
            {
                Load();
            }
            else if (ninthPlanetGames.animationFinished)
            {
                ninthPlanetGames.Hide();
                gameState = GameState.PlayerSelection;
                fluidSolver.Enabled = false;
                constantBackground.Enabled = true;
                constantBackground.Visible = true;
            }
        }

        private void OnPause(object sender, ButtonFireEventArgs args)
        {
            if ((gameState == GameState.PlayingNormalGameplay)&& !args.previouslyDown)
            {
                gameState = GameState.Paused;
            }
        }

        private void UpdatePlayerSelection(GameTime gameTime)
        {
            menu.Start(gameTime);
            if (menu.playerSelected)
            {
                mainPlayer = menu.mainPlayer;
                gameState = GameState.Menu;                
            }
        }

        private void UpdateMenu(GameTime gameTime)
        {            
            menu.Start(gameTime);
        }


        private void UpdateStartNormalGameplay(GameTime gameTime)
        {
            if (!dialogManager.IsActive)
            {
                //normalModeGamePlay.Start();
                normalModeGamePlay.Show();
                goalRenderer.Visible = true;
                goalRenderer.Enabled = true;
                menu.Visible = false;
                menu.Enabled = false;
                dialogManager.ShowDialog(DialogType.Difficulty, gameTime);
                //gameState = GameState.HowToPlay;
            }
            //if (Is1337Compliant)
            //{
            //    Is1337Compliant = false;
            //}
        }

        private void HandlePaused(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:
                    HandlePausedResult(gameTime);
                    break;
                case DialogState.Inactive:
                    break;

            }
        }

        private void HandlePausedResult(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.DialogResult)
            {
                case DialogResult.A:
                    HandlePausedSelection(gameTime);
                    break;
            }
        }

        private void HandlePausedSelection(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.SelectedOption)
            {
                case 0:
                    dialogManager.CloseDialog(gameTime);
                    gameplayTimer.Start();
                    gameState = GameState.PlayingNormalGameplay;
                    break;
                case 1:                    
                    ResetGameplay(gameTime);                    
                    break;
                case 2:
                    QuitGameplay(gameTime);
                    break;
            }
        }

        private void UpdatePaused(GameTime gameTime)
        {
            if (!dialogManager.IsActive)
            {
                normalModeGamePlay.Stop(false);
                gameplayTimer.Stop();
                dialogManager.ShowDialog(DialogType.Pause, gameTime);
            }
            else
            {
                HandlePaused(gameTime);
            }

        }

        private void ResetGameplay(GameTime gameTime)
        {             
            normalModeGamePlay.Reset(gameTime);
            normalModeGamePlay.Start();
            dialogManager.CloseDialog(gameTime);
        }

        public void HideGameplay()
        {
            normalModeGamePlay.Hide();
        }

        private void QuitGameplay(GameTime gameTime)
        {
            //ResetGameplay(gameTime);
            normalModeGamePlay.Reset(gameTime);
            HideGameplay();            
            
            dialogManager.CloseDialog(gameTime);
            gameState = GameState.Menu;
        }

        private void HandleGameOverSelection(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.SelectedOption)
            {
                case 0:
                    ResetGameplay(gameTime);
                    //dialogManager.CloseDialog(gameTime);
                    break;
                case 1:
                    QuitGameplay(gameTime);
                    break;
            }
        }

        private void HandleGameOverResult(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.DialogResult)
            {
                case DialogResult.A:
                    HandleGameOverSelection(gameTime);
                    break;
            }
        }

        private void HandleHowToPlayDialog(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:
                    switch (dialogManager.ActiveDialog.DialogResult)
                    {
                        case DialogResult.A:
                            dialogManager.CloseDialog(gameTime);
                            gameState = GameState.PlayingNormalGameplay;
                            break;
                    }
                    break;
            }
        }

        private void HandleGameOverDialog(GameTime gameTime)
        {
            switch (dialogManager.ActiveDialog.state)
            {
                case DialogState.Active:
                    HandleGameOverResult(gameTime);
                    break;
                case DialogState.Inactive:
                    
                    break;
                case DialogState.FadingIn:
                    break;
                case DialogState.FadingOut:
                    break;
            }
        }


        private void UpdateGameOver(GameTime gameTime)
        {
            if (!dialogManager.IsActive)
            {
                dialogManager.ShowDialog(DialogType.GameOverDialog, gameTime);
            }
            else
            {
                HandleGameOverDialog(gameTime);
            }
        }

        private void UpdateShutDown(GameTime gameTime)
        {
            if (!dialogManager.IsActive)
            {                
                storage.SaveHighScoresToDevice();
                storage.SaveSettingsToDevice();
                this.Exit();
            }
        }

        private void UpdatePlayingNormalGameplay(GameTime gameTime)
        {
            //normalModeGamePlay.Enabled = !dialogManager.IsActive;
            if (!dialogManager.IsActive)
            {
                if (!normalModeGamePlay.Enabled)
                {
                    normalModeGamePlay.Enabled = true;
                }
            }
        }

        private void UpdateResettingNormalGameplay(GameTime gameTime)
        {
            //levelManager.Reset();
            gameState = GameState.PlayingNormalGameplay;
        }

        private void UpdateHowToPlay(GameTime gameTime)
        {
            if (!dialogManager.IsActive)
            {
                dialogManager.ShowDialog(DialogType.Difficulty, gameTime);
            }
            else
            {
                HandleHowToPlayDialog(gameTime);
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Update(GameTime gameTime)
        {
            physicsEngine.Update(1f / 30f);
            timeSinceLastSparkle += gameTime.ElapsedGameTime;
#if XBOX            

            // Allows the game to exit
            if (Guide.IsVisible)
            {
                if (gameState == GameState.PlayingNormalGameplay)
                {
                    //normalModeGamePlay.Stop(false);
                    gameState = GameState.Paused;
                }
            }
#endif
            switch (gameState)
            {
                case GameState.RunOnce:
                    UpdateRunOnce(gameTime);                    
                    break;
                case GameState.SplashScreen:
                    UpdateSplashScreen(gameTime);
                    break;
                case GameState.Loading:
                    UpdateLoading(gameTime);
                    break;
                case GameState.PlayerSelection:
                    UpdatePlayerSelection(gameTime);                    
                    break;
                case GameState.Menu:                    
                    UpdateMenu(gameTime);
                    break;
                case GameState.StartingNormalGameplay:
                    UpdateStartNormalGameplay(gameTime);
                    break;
                case GameState.PlayingNormalGameplay:
                    UpdatePlayingNormalGameplay(gameTime);
                    break;
                case GameState.ResettingNormalGameplay:
                    UpdateResettingNormalGameplay(gameTime);
                    break;
                case GameState.Paused:
                    UpdatePaused(gameTime);
                    break;
                case GameState.GameOver:
                    UpdateGameOver(gameTime);
                    break;
                case GameState.ShutDown:
                    UpdateShutDown(gameTime);
                    break;
                case GameState.HowToPlay:
                    UpdateHowToPlay(gameTime);
                    break;
                case GameState.StorageActive:
                    break;
                case GameState.FailedPurchase:
                    OnFailedPurchase(gameTime);
                    break;
                case GameState.Credits:
                    OnCredits(gameTime);
                    break;
            }            
            base.Update(gameTime);
        }

        private void OnCredits(GameTime gameTime)
        {
            //gameState = GameState.Credits;
        }

        private void OnFailedPurchase(GameTime gameTime)
        {
            if (!popUpManager.IsActive)
            {
                popUpManager.ShowDialog(PopUpType.CannotPurchase, gameTime, OnFailedPurchaseResolved);
                
            }
        }

        private void OnFailedPurchaseResolved(PopUpResultEventArgs a)
        {
            gameState = previousGameState;
        }
        TimeSpan flickerStartTime = new TimeSpan(0,0,5);
        TimeSpan flickerCurrentTime = new TimeSpan();
        TimeSpan flickerDuration = new TimeSpan(0,0,1);
        void Update1337Flicker(GameTime gameTime)
        {
            flickerCurrentTime += gameTime.ElapsedGameTime;
            if (Is1337Compliant)
            {                
                if (flickerCurrentTime > flickerDuration)
                {
                    Is1337Compliant = false;
                    flickerStartTime = new TimeSpan(0, 0, 0, random.Next(0, 10));
                    flickerCurrentTime = new TimeSpan();
                }
            }
            else
            {                
                if (flickerCurrentTime > flickerStartTime )
                {
                    Is1337Compliant = true;
                    flickerDuration = new TimeSpan(0, 0, 0, 0,random.Next(500,1000));
                    flickerCurrentTime = new TimeSpan();
                }
            }
        }

        
        TimeSpan increment = new TimeSpan(0, 0, 0, 0, 33);
        TimeSpan time = TimeSpan.Zero;
        /// <summary>       
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.Black);
            fps = 1 / elapsed;
            base.Draw(gameTime);


        }

        void Load()
        {
            //shader1337 = new _1337Shader(this);
            
            normalGameplayRenderer = new GridScreenRenderer(this);
            recentWords = new RecentWordsRenderer(this, 5);
            letterSet = new ScrabbleSet();
            goalRenderer = new GoalRenderer(this);
            gameplayTimer = new TimerComponent(this);
            timerRenderer = new TimerRenderer(this);
            freeSpellMode = new FreeSpellMode(this);
            creditsBackground = new CreditsBackground(this);
            creditsForeground = new CreditsForeground(this);
            
            Grid grid = new Grid(alphabet);            
            letterSet.Alphabet = alphabet;
            Shorewood.wordDatabase = Content.Load<SuffixTree>("Words\\2of4brif");
            
            grid.wordDatabase = Shorewood.wordDatabase;

            grid.letterSet = letterSet;
            float gridActualWidth = 450;//(grid.width * alphabet['z'].texture.Width + alphabet['z'].texture.Width / 2) * scale;
            float gridActualHeight = (grid.height * alphabet['z'].texture.Height * 2 / 3) * scale;
            
            normalGameplayRenderer.grid = grid;
            normalModeGamePlay = new NormalGameplay(this, PlayerIndex.One, alphabet, normalGameplayRenderer, letterSet);
            gameplayTimer.AddEvent(new TimeSpan(), new OnTime(normalModeGamePlay.GameOver));
            normalModeGamePlay.wordAnimationRenderer = foundWordAnimator;
            normalGameplayRenderer.Enabled = false;            

            foundWordAnimator.scale = scale;
            foundWordAnimator.destination = new Vector2(titleSafeArea.X + 20, titleSafeArea.Y + 60);
            foundWordAnimator.destinationScale = 0.5f;
            foundWordAnimator.recentWords = recentWords;            

            normalGameplayRenderer.foundWordAnimator = foundWordAnimator;
            normalGameplayRenderer.scale = scale;
            normalGameplayRenderer.position = new Vector2(titleSafeArea.Width / 2 - gridActualWidth / 2 + titleSafeArea.X, titleSafeArea.Y);                        
            normalGameplayRenderer.Visible = false;
            
            goalRenderer.position = new Vector2(normalGameplayRenderer.position.X + gridActualWidth + 10, titleSafeArea.Y + 20);
           
            foundWordAnimator.startPositionOffset = normalGameplayRenderer.position;
            
            recentWords.position = foundWordAnimator.destination;
            recentWords.startOffsetPosition = foundWordAnimator.startPositionOffset;
            recentWords.scale = scale;

            normalModeGamePlay.Enabled = false;

            isLoaded = true;
            
            Components.Add(constantBackground);
            
            Components.Add(normalModeGamePlay);
            Components.Add(normalGameplayRenderer);
            Components.Add(goalRenderer);    
            
                       
            Components.Add(recentWords);
            
            Components.Add(scoreBox);
            
            Components.Add(foundWordAnimator);
            Components.Add(timerRenderer);
            Components.Add(freeSpellMode);
            bloom.Settings = BloomSettings.PresetSettings[(int)BloomType.Blurry];
            Components.Add(bloom);
            Components.Add(creditsBackground);
            Components.Add(particleSystem);
            Components.Add(creditsForeground);
            Components.Add(menu);
            Components.Add(dialogManager);
            Components.Add(popUpManager);
            Components.Add(storageScreen);
            Components.Add(gameplayTimer);            
            
            bloom.Enabled = false;
            bloom.Visible = false;
            levelManager = new NormalGameplayLevelManager();
            normalModeGamePlay.levelManager = levelManager;
            goalRenderer.levelManager = levelManager;
            
            GC.Collect();
            soundTrack.Start();
        }
    }
}
