using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Localization;

namespace IETGames.Shorewood.Goals
{
    public class BackwardsWordCountGoal : WordCountGoal        
    {
        public BackwardsWordCountGoal(int numberOfWords, int wordLength, TimeSpan goalStartTime)
            : base(numberOfWords, wordLength, goalStartTime)
        {
            if (wordLength > 0)
            {
                gameString = GameStrings.GoalSpecificLengthBackwards;
            }
            else
            {
                gameString = GameStrings.GoalAnyBackwards;
            }
            Reset(numberOfWords, wordLength);
        }

        public override bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            if (builder.isReversed)
            {
                return base.CheckGoal(builder, countGoal);
            }
            else
            {
                return false;
            }
        }

    }
}