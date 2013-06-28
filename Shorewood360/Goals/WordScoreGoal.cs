using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Localization;

namespace IETGames.Shorewood.Goals
{
    public class WordScoreGoal:IGoal
    {
        bool isGoalCompleted = false;
        bool hasUpdated = true;
        float goalStatus = 0f;
        int goalTotalNumberOfWords = 0;
        int score = 0;
        int currentCount = 0;

        protected IETGames.Shorewood.GoalRenderer.GoalBox goalBox;
        protected GameStrings gameString;
        protected StringBuilder goalText;
        protected StringBuilder goalTitle;
        protected TimeSpan goalStartTime;

        public WordScoreGoal(int score, int count, TimeSpan goalStartTime)
        {
            this.score = score;
            goalTotalNumberOfWords = count;
            gameString = GameStrings.GoalPoints;
            this.goalStartTime = goalStartTime;
            Reset();
        }

        #region IGoal Members

        public bool IsGoalCompleted
        {
            get
            { 
                return isGoalCompleted; 
            }
        }

        public bool HasUpdated
        {
            get
            {
                return hasUpdated;
            }
            set
            {
                hasUpdated = value;
            }
        }

        public bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            if (isGoalCompleted)
            {
                return true;
            }
            else if ( builder.Points > score)
            {
                if (countGoal)
                {
                    currentCount++;
                    goalStatus = (float)currentCount / (float)goalTotalNumberOfWords;
                    if (currentCount == goalTotalNumberOfWords)
                    {
                        isGoalCompleted = true;
                    }
                    hasUpdated = true;
                }
                return true;
            }
            return false;
        }

        public StringBuilder GoalText
        {
            get 
            {
                return goalText;
            }
        }

        public StringBuilder GoalTitle
        {
            get 
            {
                return goalTitle;
            }
        }

        public float GoalStatus
        {
            get 
            {
                return goalStatus;
            }
        }

        public int Count
        {
            get 
            {
                return goalTotalNumberOfWords;
            }
        }

        public void Reset()
        {
            hasUpdated = true;
            isGoalCompleted = false;
            currentCount = 0;
            goalText = new StringBuilder(Shorewood.localization[Shorewood.language][GameStrings.GoalPoints].ToString());
            goalText = goalText.Replace("<X>", goalTotalNumberOfWords.ToString());
            goalText = goalText.Replace("<Y>", score.ToString());
            goalStatus = 0;
            
        }

        public TimeSpan GoalStartTime
        {
            get { return goalStartTime; }
        }

        public GoalRenderer.GoalBox GoalBox
        {
            get
            {
                return goalBox;
            }
            set
            {
                goalBox = value;
            }
        }

        public virtual void Activate()
        {
            Reset();
        }

        public virtual void Deactivate()
        {
            Reset();
        }

        #endregion
    }
}
