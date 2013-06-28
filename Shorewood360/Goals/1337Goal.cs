using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IETGames.Shorewood;

namespace IETGames.Shorewood.Goals
{
    public class _1337Goal:WordCountGoal
    {
        public _1337Goal(int numberOfWords, int wordLength, TimeSpan goalStartTime)
            :base(numberOfWords, wordLength, goalStartTime)
        {
            gameString = IETGames.Shorewood.Localization.GameStrings.Goal1337;
        }

        public override void Activate()
        {
            if (!Shorewood.Is1337Compliant)
            {
                Shorewood.Is1337Compliant = true;
            }
            base.Activate();
        }

        public override void Deactivate()
        {
            if (Shorewood.Difficulty != Difficulty._1337)
            {
                Shorewood.Is1337Compliant = false;
            }
            base.Deactivate();
        }
    }
}