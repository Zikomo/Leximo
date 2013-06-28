using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IETGames.Shorewood.Localization;

namespace IETGames.Shorewood.Goals
{
    public class PalindromeGoal : WordCountGoal
    {
        public PalindromeGoal(int numberOfWords, int wordLength, TimeSpan goalStartTime)
            : base(1, 3, goalStartTime)
        {
            if (wordLength > 0)
            {
                gameString = GameStrings.GoalPalindrome;
            }
            else
            {
                gameString = GameStrings.GoalPalindrome;
            }
            Reset(numberOfWords, wordLength);
        }

        public override bool CheckGoal(WordBuilder builder, bool countGoal)
        {
            if (builder.IsPalindrome)
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