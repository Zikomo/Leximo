using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Localization;

namespace IETGames.Shorewood.Goals
{
    public class SpecificWordGoal:IGoal
    {
        protected bool isGoalCompleted = false;
        protected bool hasUpdated = true;
        

        protected IETGames.Shorewood.GoalRenderer.GoalBox goalBox;
        protected GameStrings gameString;
        protected StringBuilder goalText;
        protected StringBuilder goalTitle;
        protected TimeSpan goalStartTime;
        protected StringBuilder word;

        public SpecificWordGoal(StringBuilder word, TimeSpan goalStartTime)
        {
            this.goalStartTime = goalStartTime;
            this.word = word;
            gameString = GameStrings.GoalSpecific;
            
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

        public virtual bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            word.EnsureCapacity(60);
            if (!builder.isReversed)
            {
                if (builder.CurrentString.Equals(word))
                {
                    if (countGoal)
                    {
                        isGoalCompleted = true;
                        hasUpdated = true;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (builder.CurrentReversedString.Equals(word))
                {
                    if (countGoal)
                    {
                        isGoalCompleted = true;
                        hasUpdated = true;
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
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
                if (IsGoalCompleted)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Count
        {
            get 
            {
                return 1;
            }
        }

        public void Reset()
        {
            isGoalCompleted = false;
            hasUpdated = true;
            goalText = new StringBuilder(Shorewood.localization[Shorewood.language][gameString].ToString());
            goalText = goalText.Replace("<X>", word.ToString().ToUpper());
        }
        
        public TimeSpan GoalStartTime
        {
            get { throw new NotImplementedException(); }
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
            foreach (char letter in word.ToString())
            {
                Shorewood.letterSet.InjectedLetterList.Add(letter);
            }
        }

        public virtual void Deactivate()
        {
            Reset();
            foreach (char letter in word.ToString())
            {
                Shorewood.letterSet.InjectedLetterList.Remove(letter);
            }
        }

        #endregion
    }
}