using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Localization;

namespace IETGames.Shorewood.Goals
{
    public class WordCountGoal:IGoal
    {
        bool isGoalCompleted = false;
        bool hasUpdated = true;
        float goalStatus = 0f;
        int goalTotalNumberOfWords = 0;
        int wordLength = 0;
        int currentCount = 0;
        protected IETGames.Shorewood.GoalRenderer.GoalBox goalBox;
        protected GameStrings gameString;
        protected StringBuilder goalText;
        protected StringBuilder goalTitle;
        protected TimeSpan goalStartTime;
        protected bool is1337;

        public WordCountGoal(int numberOfWords, int wordLength, TimeSpan goalStartTime)
        {            
            this.goalStartTime = goalStartTime;
            gameString = GameStrings.GoalCount;
            Reset(numberOfWords, wordLength);
            goalTitle = new StringBuilder();
            is1337 = false;
        }

        public WordCountGoal(int numberOfWords, int wordLength, TimeSpan goalStartTime, bool is1337Compliant)
        {

        }

        #region IGoal Members

        public bool IsGoalCompleted
        {
            get 
            { 
                return isGoalCompleted; 
            }
        }

        public virtual void Reset(int numberOfWords, int wordLength)
        {
            goalText = new StringBuilder(Shorewood.localization[Shorewood.language][gameString].ToString());
            goalText = goalText.Replace("<X>", numberOfWords.ToString());
            goalText = goalText.Replace("<Y>", wordLength.ToString());
            this.goalTotalNumberOfWords = numberOfWords;
            this.wordLength = wordLength;
            isGoalCompleted = false;
            hasUpdated = true; 
            goalStatus = 0;
            currentCount = 0;
        }

        public virtual bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            if (isGoalCompleted)
            {
                return true;
            }
            else if ((wordLength == 0) || (builder.letters.Count >= wordLength))
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

        public float GoalStatus
        {
            get 
            {
                return goalStatus;
            }
        }

        public StringBuilder GoalTitle
        {
            get { throw new NotImplementedException(); }
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

        public virtual void Reset()
        {
            Reset(goalTotalNumberOfWords, wordLength);
        }

        public int Count
        {
            get
            {
                return goalTotalNumberOfWords;
            }
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