using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Goals;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood
{
    public class LevelManager
    {
        public struct Level
        {
            public Queue<IGoal> queuedGoals;
            public Queue<IGoal> originalGoals;
            public List<IGoal> activeGoals;
            public TimeSpan levelDuration;
            public float speed;
            public TimeSpan goalFrequency;
            public TimeSpan goalRewardTime;
            public LevelManager manager;
            public IGoal startingGoal;
            public bool is1337;
            public Level(float speed, LevelManager manager)
            {
                this.activeGoals = new List<IGoal>(20);
                this.speed = speed;
                this.goalFrequency = new TimeSpan(0,0,30);
                this.queuedGoals = new Queue<IGoal>(20);
                this.originalGoals = new Queue<IGoal>(20);
                this.levelDuration = new TimeSpan(0, 3, 31);
                this.manager = manager;
                this.startingGoal = null;
                this.goalRewardTime = new TimeSpan(0, 0, 30);
                this.is1337 = false;
                
            }

            public void AddGoal(IGoal goal)
            {
                goal.GoalBox = Shorewood.goalRenderer.GetGoalBox();
                originalGoals.Enqueue(goal);
                //Shorewood.gameplayTimer.AddEvent(goal.GoalStartTime, new IETGames.Shorewood.Utilities.OnTime(manager.GetNextGoal), goal);
                
                queuedGoals.Enqueue(goal);
            }

            public void AddStartGoal(IGoal goal)
            {
                goal.GoalBox = Shorewood.goalRenderer.GetGoalBox();
                this.startingGoal = goal;
                //originalGoals.Enqueue(goal);
                //Shorewood.gameplayTimer.AddEvent(goal.GoalStartTime, new IETGames.Shorewood.Utilities.OnTime(manager.GetNextGoal), goal);
            }

            public void IncreaseFrequency(GameTime gameTime, object eventArgument)
            {
                
            }
        }

        //public int currentLevel = 1;
        public List<Level> predeterminedLevels;
        protected int levelCount = 20;
        

        public LevelManager(int levelCount)
        {
            predeterminedLevels = new List<Level>(levelCount);        
        }

        public List<Level> PredeterminedLevels
        {
            get
            {
                return predeterminedLevels; 
            }
        }
        List<IGoal> goalCleaner = new List<IGoal>(10);
        public void Update(GameTime gameTime)
        {
            foreach (var goal in CurrentLevelGoals)
            {
                if (goal.IsGoalCompleted)
                {
                    goalCleaner.Add(goal);
                }
            }

            foreach (var goal in goalCleaner)
            {
                goal.Deactivate();
                goal.GoalBox.Discard();
                //CurrentLevelGoals.Remove(goal);
                Shorewood.goalRenderer.ForceUpdateOnce();
                Shorewood.gameplayTimer.CountdownTime += predeterminedLevels[(int)Shorewood.Difficulty].goalRewardTime;                
            }
            goalCleaner.Clear();
            if (CurrentLevelGoals.Count == 0)
            {
                if (!Shorewood.freeSpellMode.Visible)
                {
                    Shorewood.freeSpellMode.Show();
                }
            }
        }

        public bool CheckIfWordMeetsGoal(WordBuilder builder, bool countGoal)
        {
            if (Shorewood.Difficulty != Difficulty.FreeSpell)
            {
                bool wordMeetsGoal = false;
                List<IGoal> currentGoals = CurrentLevelGoals;
                for (int i = 0; i < currentGoals.Count; i++)
                {
                    IGoal goal = currentGoals[i];
                    if (!goal.IsGoalCompleted && !goal.GoalBox.IsDiscarding)
                    {
                        if (goal.CheckGoal(builder, countGoal))
                        {
                            wordMeetsGoal = true;
                        }
                    }
                }

                return wordMeetsGoal;
            }
            else
            {
                return true;
            }
        }

        public void Start()
        {
            Shorewood.gameplayTimer.ClearIntervalEvents();
            Shorewood.gameplayTimer.AddIntervalEvent(new TimeSpan(0, 0, 1), Shorewood.timerRenderer.OnTimerChange, null);
            Shorewood.gameplayTimer.AddIntervalEvent(predeterminedLevels[(int)Shorewood.Difficulty].goalFrequency, new IETGames.Shorewood.Utilities.OnTime(GetNextGoal), null);
            Shorewood.gameplayTimer.Duration = predeterminedLevels[(int)Shorewood.Difficulty].levelDuration;
            if (Shorewood.Difficulty != Difficulty.FreeSpell)
            {
                Shorewood.timerRenderer.NextChallenge = predeterminedLevels[(int)Shorewood.Difficulty].goalFrequency;
                predeterminedLevels[(int)Shorewood.Difficulty].startingGoal.GoalBox.Reset();
                predeterminedLevels[(int)Shorewood.Difficulty].startingGoal.Activate();

                CurrentLevelGoals.Add(predeterminedLevels[(int)Shorewood.Difficulty].startingGoal);
            }
        }

        public void Reset()
        {            
            RandomGoalGenerator.Reset(10);
            CurrentLevelGoals.Clear();
            predeterminedLevels[(int)Shorewood.Difficulty].activeGoals.Clear();
            predeterminedLevels[(int)Shorewood.Difficulty].queuedGoals.Clear();
            predeterminedLevels[(int)Shorewood.Difficulty].startingGoal.Reset();
            foreach (var goal in predeterminedLevels[(int)Shorewood.Difficulty].originalGoals)
            {
                goal.Reset();
                predeterminedLevels[(int)Shorewood.Difficulty].queuedGoals.Enqueue(goal);
            }
        }

        public void GetNextGoal(GameTime gameTime, object goal)
        {
            if (Shorewood.Difficulty != Difficulty.FreeSpell)
            {
                if (Shorewood.freeSpellMode.Visible)
                {
                    Shorewood.freeSpellMode.Hide();
                }
                if (CurrentLevelGoals.Count < 3)
                {
                    if (goal == null)
                    {
                        if ((predeterminedLevels[(int)Shorewood.Difficulty].queuedGoals.Count > 0))
                        {
                            IGoal nextGoal = predeterminedLevels[(int)Shorewood.Difficulty].queuedGoals.Dequeue();
                            nextGoal.GoalBox.Reset();
                            nextGoal.Activate();
                            CurrentLevelGoals.Add(nextGoal);
                        }
                        else
                        {
                            IGoal nextGoal = RandomGoalGenerator.GenerateGoal();
                            nextGoal.Activate();
                            nextGoal.GoalBox = Shorewood.goalRenderer.GetGoalBox();
                            //nextGoal.GoalBox.Reset();                        
                            nextGoal.GoalBox.IsFading = true;
                            CurrentLevelGoals.Add(nextGoal);
                        }
                    }
                    else
                    {
                        if ((predeterminedLevels[(int)Shorewood.Difficulty].queuedGoals.Count > 0))
                        {
                            IGoal nextGoal = (IGoal)goal;
                            CurrentLevelGoals.Add(nextGoal);
                        }
                    }
                    Shorewood.timerRenderer.NextChallenge = predeterminedLevels[(int)Shorewood.Difficulty].goalFrequency;
                }
            }
        }

        public bool AllGoalsCompleted
        {
            get
            {
                bool levelStatus = true;
                foreach (var goal in CurrentLevelGoals)
                {
                    levelStatus &= goal.IsGoalCompleted;
                }
                return levelStatus;
            }
        }

        public List<IGoal> CurrentLevelGoals
        {
            get
            {
                return predeterminedLevels[(int)Shorewood.Difficulty].activeGoals;
            }
        }

        public float Speed
        {
            get
            {
                return predeterminedLevels[(int)Shorewood.Difficulty].speed;
            }
        }

        //public int DistinctLevels
        //{
        //    get
        //    {
        //        return predeterminedLevels.Count;
        //    }
        //}
    }
}