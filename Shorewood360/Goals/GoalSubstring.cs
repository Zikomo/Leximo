using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood.Goals
{
    class GoalSubstring:SpecificWordGoal
    {
        public GoalSubstring(StringBuilder word, TimeSpan goalStartTime)
            :base(word, goalStartTime)
        {
            this.word = word;
            this.gameString = IETGames.Shorewood.Localization.GameStrings.GoalContainsWord;
        }

        public override bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            if (!builder.isReversed)
            {
                if (builder.CurrentString.ToString().Contains(word.ToString()) && word.Length < builder.CurrentString.Length)
                {
                    if (countGoal)
                    {
                        isGoalCompleted = true;
                        hasUpdated = true;
                    }
                    return true;
                }
            }
            else
            {
                if (builder.CurrentReversedString.ToString().Contains(word.ToString()) && word.Length < builder.CurrentReversedString.Length)
                {
                    if (countGoal)
                    {
                        isGoalCompleted = true;
                        hasUpdated = true;
                    }
                    return true;
                }
            }
            
            
            return false;            
        }        
    }
}