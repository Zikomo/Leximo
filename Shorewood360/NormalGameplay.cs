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
using ListPool;
using System.Diagnostics;
using IETGames.Shorewood.Threads;
using IETGames.Shorewood.Storage;
using System.Text;
using IETGames.Shorewood.Input;


namespace IETGames.Shorewood
{
    public class NormalGameplay : Microsoft.Xna.Framework.GameComponent
    {
        #region Types
        public enum NormalGameplayState {CurrentLetterActive, FindingWords, WordsFound, NoMoreWords, NewGoals, WordSearch, GameOver};
        #endregion 

        #region Private Variables
        protected Letter nextLetter;        
        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;
        Vector2 previousLeftThumbStickPositionOne;
        protected bool isGameOver = false;
        Shorewood game;
        EventHandler<ButtonFireEventArgs> onLeft;
        EventHandler<ButtonFireEventArgs> onRight;
        EventHandler<ButtonFireEventArgs> onA;
        EventHandler<ButtonFireEventArgs> onTrigger;

        #endregion

        #region Public Variables
        public Letter currentLetter;
        public List<FloatingPoints> floatingPoints;
        public Dictionary<char, Letter> alphabet;
        public bool isPaused = false;
        public float startingVelocity = 1010;
        public float currentVelocity = 1010;
        public float lastLetterUpdate = 0;
        public bool isStarted = false;
        public ILetterSet letterSet;
        public Vector2 previousCurrentLetterPosition;
        public GridRenderer gridRenderer;
        //public PlayerIndex playerIndex;
        public FoundWordAnimationRenderer wordAnimationRenderer;
        public NormalGameplayState gameplayState = NormalGameplayState.GameOver;
        public LevelManager levelManager;
        public int maximumQueuedWords = 10;
        #endregion

        #region Constructors
        public NormalGameplay(Game game, PlayerIndex player, Dictionary<char, Letter> alphabet, GridRenderer renderer, ILetterSet letterSet)
            : base(game)
        {
            //this.player = player;
            this.game = (Shorewood)game;
            this.alphabet = alphabet;
            this.gridRenderer = renderer;
            this.letterSet = letterSet;
            this.Enabled = false;
            this.onA = new EventHandler<ButtonFireEventArgs>(OnA);
            this.onLeft = new EventHandler<ButtonFireEventArgs>(OnLeft);
            this.onRight = new EventHandler<ButtonFireEventArgs>(OnRight);
            this.onTrigger = new EventHandler<ButtonFireEventArgs>(OnTrigger);
            AddEvents();
        }
        #endregion 


        public override void Initialize()
        {
            previousLeftThumbStickPositionOne = new Vector2(0, 0);            
            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(Shorewood.mainPlayer);

            base.Initialize();
        }

        public void Reset(GameTime gameTime)
        {
            Shorewood.freeSpellMode.Hide();
            PredictiveTextAnalyzer.Stop();
            PredictiveTextAnalyzer.ClearSuggestedLetters();
            letterSet.Reset();            
            Shorewood.levelManager.Reset();
            Shorewood.scoreBox.Reset();
            Shorewood.recentWords.Reset();
            Shorewood.gameplayTimer.Reset();
            Shorewood.goalRenderer.ForceUpdateOnce(); 
            
            gridRenderer.grid.ClearGrid();
            isGameOver = false;
            Shorewood.gameState = GameState.ResettingNormalGameplay;
            gameplayState = NormalGameplayState.GameOver;
            currentLetter = Letter.EmptyLetter;
            lastLetterUpdate = 0;           
            Shorewood.Is1337Compliant = false;

        }

        private void AddEvents()
        {
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickLeft, onLeft);
            Shorewood.inputHandler.AddEvent(Buttons.DPadLeft, onLeft);
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickRight, onRight);
            Shorewood.inputHandler.AddEvent(Buttons.DPadRight, onRight);
            Shorewood.inputHandler.AddEvent(Buttons.A, onA);
            Shorewood.inputHandler.AddEvent(Buttons.LeftTrigger, onTrigger);
            Shorewood.inputHandler.AddEvent(Buttons.RightTrigger, onTrigger);
        }

        private void RemoveEvents()
        {
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickLeft, onLeft);
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadLeft, onLeft);
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickRight, onRight);
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadRight, onRight);
            Shorewood.inputHandler.RemoveEvent(Buttons.A, onA);
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftTrigger, onTrigger);
            Shorewood.inputHandler.RemoveEvent(Buttons.RightTrigger, onTrigger);
        }


        public void Show()
        {
            this.Enabled = true;
            this.gridRenderer.Visible = true;
            this.gridRenderer.Enabled = true;
            Shorewood.scoreBox.Visible = true;
            Shorewood.scoreBox.Enabled = true;
            Shorewood.recentWords.Enabled = true;
            Shorewood.recentWords.Visible = true;
            Shorewood.timerRenderer.Visible = true;
            Shorewood.timerRenderer.Enabled = true;
            //AddEvents();
            
        }

        public void Hide()
        {
            this.Enabled = false;
            this.gridRenderer.Visible = false;
            this.gridRenderer.Enabled = false;
            Shorewood.scoreBox.Visible = false;
            Shorewood.scoreBox.Enabled = false;
            Shorewood.recentWords.Enabled = false;
            Shorewood.recentWords.Visible = false;
            Shorewood.timerRenderer.Visible = false;
            Shorewood.timerRenderer.Enabled = false;
            Shorewood.goalRenderer.Visible = false;
            Shorewood.goalRenderer.Enabled = false;
            Shorewood.freeSpellMode.Hide();
            PredictiveTextAnalyzer.Stop();
            gameplayState = NormalGameplayState.GameOver;            
        }

        public void Start()
        {            
            levelManager.Start();
            Shorewood.gameplayTimer.Reset();
            Shorewood.gameplayTimer.Start();
            PredictiveTextAnalyzer.Start();
            letterSet.InjectedLetterList.Clear();
            nextLetter = letterSet.GetRandomLetter();
            ChangeCurrentLetterToRandom(null);
            Show();
            
            isStarted = true;
            ChangeVelocity(levelManager.Speed);
            
        }

        public void OnLeft(object sender, ButtonFireEventArgs args)
        {
            if ((!args.previouslyDown) && gameplayState == NormalGameplayState.CurrentLetterActive 
                && !Shorewood.dialogManager.IsActive )
            {
                MoveLeft();
            }
        }

        public void OnRight(object sender, ButtonFireEventArgs args)
        {
            if (!args.previouslyDown && gameplayState == NormalGameplayState.CurrentLetterActive
                && !Shorewood.dialogManager.IsActive)
            {
                MoveRight();
            }
        }

        public void OnTrigger(object sender, ButtonFireEventArgs args)
        {
            if (!args.previouslyDown && gameplayState == NormalGameplayState.CurrentLetterActive
                && !Shorewood.dialogManager.IsActive)
            {
                if (gridRenderer.grid.validWords.Count > 0)
                {
                    gameplayState = NormalGameplayState.FindingWords;
                }
            }
        }

        public void OnA(object sender, ButtonFireEventArgs args)
        {
            if (!args.previouslyDown && gameplayState == NormalGameplayState.CurrentLetterActive && !Shorewood.dialogManager.IsActive
                && !Shorewood.dialogManager.IsActive)
            {
                DropLetter(args.gameTime);
            }
        }


        public void Stop(bool hide)
        {
            Shorewood.gameplayTimer.Stop();
            if (hide)
            {
                
                Hide();
            }
            else
            {
                this.Enabled = false;
            }            
        }
        Stopwatch timer = new Stopwatch();
        protected void UpdateCurrentLetter(GameTime gameTime)
        {
            gridRenderer.currentLetter = currentLetter;
            if ((isStarted) && (!isGameOver))
            {
                float timeSinceLastUpdate = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (lastLetterUpdate > currentVelocity)
                {
                    if (MoveDown(gameTime))
                    {                        
                        return;
                    }
                }  
                lastLetterUpdate += timeSinceLastUpdate;
            }
        }

        public bool CheckMoveDown(GameTime gameTime)
        {
            bool rtnValue = false;
            if (currentLetter.gridPosition.Y + 1>= gridRenderer.grid.height)
            {
                return rtnValue;
            }
            rtnValue = gridRenderer.grid.GetLetter(this.currentLetter.gridPosition.X, currentLetter.gridPosition.Y + 1) == Letter.EmptyLetter;
            if ((currentLetter.gridPosition.Y == 0 && currentLetter.gridPosition.X == Math.Round(gridRenderer.grid.width / 2))  && !rtnValue)
            {
                GameOver(gameTime, null);
            }
            return rtnValue;
            
        }        

        public bool MoveUp()
        {
            currentLetter.gridPosition.Y--;
            if (currentLetter.gridPosition.Y < 0)
            {
                currentLetter.gridPosition.Y = 0;
            }
            gridRenderer.grid.PositionLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y, currentLetter);
            return gridRenderer.grid.GetLetter(this.currentLetter.gridPosition.X, currentLetter.gridPosition.Y) == Letter.EmptyLetter;
        }

        public bool MoveDown(GameTime gameTime)
        {            
            if (!CheckMoveDown(gameTime))
            {
                HandleLetterDrop(currentLetter, gameTime);
                return true;
            }
            else
            {                
                gridRenderer.grid.PositionLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y, Letter.EmptyLetter);
                currentLetter.gridPosition.Y++;
                gridRenderer.grid.PositionLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y, currentLetter);
                //changeLetter = false;
            }
            lastLetterUpdate = 0;
            return false;           
        }


        public bool TryMoveLeft()
        {
            return gridRenderer.grid.GetLetter(currentLetter.gridPosition.X - 1, currentLetter.gridPosition.Y) == Letter.EmptyLetter;
        }

        public bool TryMoveRight()
        {
            return gridRenderer.grid.GetLetter(currentLetter.gridPosition.X + 1, currentLetter.gridPosition.Y) == Letter.EmptyLetter;
        }


        public bool MoveLeft()
        {
            if (TryMoveLeft())
            {
                if (currentLetter.gridPosition.X != 0)
                {
                    gridRenderer.grid.EraseLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y);
                    currentLetter.gridPosition.X--;
                    gridRenderer.grid.PositionLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y, currentLetter);
                    return true;
                }                
                return true;
            }
            return false;
        }

        public bool MoveRight()
        {
            if (TryMoveRight())
            {

                if (currentLetter.gridPosition.X < gridRenderer.grid.width - 1)
                {                    
                    gridRenderer.grid.EraseLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y);
                    currentLetter.gridPosition.X++;
                    gridRenderer.grid.PositionLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y, currentLetter);
                    return true;
                }                
                return true;
            }
            return false;
        }

        public void DropLetter(GameTime gameTime)
        {
            Letter dropLetter = currentLetter;
            gridRenderer.grid.EraseLetter((int)currentLetter.gridPosition.X, (int)currentLetter.gridPosition.Y);
            if (CheckMoveDown(gameTime))
            {
                for (dropLetter.gridPosition.Y = (int)currentLetter.gridPosition.Y; dropLetter.gridPosition.Y < gridRenderer.grid.height - 1; dropLetter.gridPosition.Y++)
                {
                    if (gridRenderer.grid.GetLetter(currentLetter.gridPosition.X, dropLetter.gridPosition.Y + 1) != Letter.EmptyLetter)
                    {                       
                        break;
                    }                    
                }
            }
            HandleLetterDrop(dropLetter, gameTime);            
        }

        private void HandleLetterDrop(Letter letter, GameTime gameTime)
        {
            gridRenderer.grid.ResetGlowers();
            gridRenderer.grid.SetLetter(letter.gridPosition.X, letter.gridPosition.Y, letter);
            previousCurrentLetterPosition = gridRenderer.grid.GetLetter(letter.gridPosition.X, letter.gridPosition.Y).position;
            gameplayState = NormalGameplayState.WordSearch;
            Shorewood.clink.Play();
            //ChangeCurrentLetterToRandom(gameTime);
        }

        public void GameOver(GameTime gameTime, object nothing)
        {
            isGameOver = true;
            Shorewood.gameState = GameState.GameOver;
            int score = gridRenderer.grid.currentScore;
            TimeSpan duration = Shorewood.gameplayTimer.ElapsedTime;
            ScoreEntry highScore = new ScoreEntry();
            highScore.Difficulty = Shorewood.Difficulty;
            highScore.Duration = duration;
            highScore.Score = Shorewood.scoreBox.CurrentScore;
#if XBOX
            if ((Gamer.SignedInGamers.Count > 0) && Gamer.SignedInGamers[Shorewood.mainPlayer] != null)
            {
                highScore.GamerTag = Gamer.SignedInGamers[Shorewood.mainPlayer].Gamertag;
                GamerEntry gamer = new GamerEntry();
                gamer.GamerPicture = Gamer.SignedInGamers[Shorewood.mainPlayer].GetProfile().GamerPicture;
                gamer.gamerTag = highScore.GamerTag;
                if (!Shorewood.storage.HighScores.Gamers.Contains(gamer))
                {
                    Shorewood.storage.HighScores.Gamers.Add(gamer);
                }
            }
#else
            highScore.GamerTag = "Zikomo";
#endif
            List<ScoreEntry> scores = Shorewood.storage.HighScores.GetList(Shorewood.Difficulty);
            scores.Add(highScore);
            scores.Sort();
            if (scores.Count > 10)
            {
                scores.RemoveRange(10, scores.Count - 10);
            }
            Shorewood.storage.SaveHighScoresToDevice();
            gameplayState = NormalGameplayState.GameOver;
        }


        protected virtual void ChangeCurrentLetterToRandom(GameTime now)
        {
            Letter letter = new Letter();
            letter = nextLetter;            
            letter.gridPosition.X = (float)Math.Round(gridRenderer.grid.width / 2);
            letter.gridPosition.Y = 0;
            gridRenderer.grid.PositionLetter((int)letter.gridPosition.X, (int)letter.gridPosition.Y, letter); 
           
            currentLetter = letter;
            gridRenderer.currentLetter = currentLetter;

            if (now != null)
            {
                lastLetterUpdate = (float)now.ElapsedGameTime.TotalMilliseconds;
            }
            nextLetter = letterSet.GetRandomLetter();
            gridRenderer.nextLetter = nextLetter;
            gameplayState = NormalGameplayState.CurrentLetterActive;
        }

        private void UpdatePoints(GameTime gameTime)
        {
            for (int i = 0; i < floatingPoints.Count; i++)
            {
                if (!floatingPoints[i].isAlive)
                {
                    ShorewoodPool.floatingPointsPool.Return(floatingPoints[i]);
                    floatingPoints.RemoveAt(i);
                }
            }

            foreach (var point in floatingPoints)
            {
                point.Update(gameTime);
            }
        }

        TimeSpan wordUpdateTrigger = new TimeSpan();

        private void UpdateWords(GameTime gameTime)
        {
            if ((gridRenderer.grid.validWords.Count > 0)&& (wordUpdateTrigger > new TimeSpan(0,0,0,0,500)))
            {
                if (Shorewood.gameplayTimer.IsStarted)
                {
                    Shorewood.gameplayTimer.Stop();
                }
                
                WordBuilder wordCheck = gridRenderer.grid.validWords.Dequeue();//gridRenderer.grid.validWords[gridRenderer.grid.validWords.Count - 1];
                levelManager.CheckIfWordMeetsGoal(wordCheck, true);
                WordBuilder word = wordCheck;
                word = gridRenderer.grid.ScoreValidWord(word);
                Shorewood.foundWordAnimator.Add(word, TimeSpan.Zero);

                garbageLetters.AddRange(word.letters);
                Shorewood.goalRenderer.ForceUpdateOnce();
                
                gridRenderer.StartRipple(Vector2.Zero, Vector2.Zero, (float)gameTime.TotalGameTime.TotalMilliseconds, 250);
                
                wordUpdateTrigger = new TimeSpan();
            }
            else if (gridRenderer.grid.validWords.Count == 0)
            {
                gameplayState = NormalGameplayState.NoMoreWords;
            }
            
        }

        public void ChangeVelocity(float velocity)
        {
            velocity = MathHelper.Clamp(velocity, 0, 1);
            currentVelocity = MathHelper.Clamp(startingVelocity - velocity*(startingVelocity), 500 / gridRenderer.grid.height, startingVelocity);
            Shorewood.constantBackground.ParticleVelocity = MathHelper.Clamp(velocity, 0.01f, 1f);
        }

        public void UpdateGoals(GameTime gameTime)
        {
            gameplayState = NormalGameplayState.CurrentLetterActive;
            Shorewood.goalRenderer.ForceUpdateOnce();
            //gridRenderer.grid.CheckForChanges();
            
        }

        public void UpdateGoalDialog(GameTime gameTime)
        {
            if (Shorewood.dialogManager.IsActive)
            {
                switch (Shorewood.dialogManager.ActiveDialog.DialogResult)
                {
                    case DialogResult.A:                        
                        Shorewood.dialogManager.CloseDialog(gameTime);
                        break;
                }
            }
            else
            {
                ChangeCurrentLetterToRandom(gameTime);
                //changeLetter = false;
                previousKeyboardState = Keyboard.GetState();
                previousGamePadState = GamePad.GetState(Shorewood.mainPlayer);
            }
        }

        public void UpdateVelocity(GameTime gameTime)
        {
            float durationMilliseconds = (float)Shorewood.gameplayTimer.Duration.TotalMilliseconds;
            float currentTimeMilliseconds = (float)Shorewood.gameplayTimer.CountdownTime.TotalMilliseconds;
            ChangeVelocity(MathHelper.Clamp(1 - currentTimeMilliseconds / durationMilliseconds, 0, 1));
        }

        List<Letter> garbageLetters = new List<Letter>(400);
        public void CleanUp()
        {            
            gridRenderer.grid.CleanUpLetters(garbageLetters);
            garbageLetters.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isGameOver && !Shorewood.dialogManager.IsActive)
            {
                levelManager.Update(gameTime);
                UpdateVelocity(gameTime);
                wordUpdateTrigger += gameTime.ElapsedGameTime;
                switch (gameplayState)
                {
                    case NormalGameplayState.CurrentLetterActive:
                        if (!Shorewood.gameplayTimer.IsStarted)
                        {
                            Shorewood.gameplayTimer.Start();
                        }
                        UpdateCurrentLetter(gameTime);
                        break;
                    case NormalGameplayState.FindingWords:
                        UpdateWords(gameTime);
                        break;
                    case NormalGameplayState.WordsFound:
                        UpdateWords(gameTime);
                        break;
                    case NormalGameplayState.NoMoreWords:
                        CleanUp();
                        UpdateGoals(gameTime);
                        break;
                    case NormalGameplayState.NewGoals:
                        UpdateGoalDialog(gameTime);
                        break;
                    case NormalGameplayState.WordSearch:
                        if (!gridRenderer.grid.isSearching)
                        {
                            ChangeCurrentLetterToRandom(gameTime);
                        }
                        break;

                }
                gridRenderer.grid.ValidateGrid(gameTime, currentLetter);
                base.Update(gameTime);
            }
            else
            {
                if (Shorewood.gameplayTimer.IsStarted)
                {
                    Shorewood.gameplayTimer.Stop();
                }
            }
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (!Enabled)
            {
                previousGamePadState = GamePad.GetState(Shorewood.mainPlayer);
                previousKeyboardState = Keyboard.GetState();
            }
            base.OnEnabledChanged(sender, args);
        }
    }
}