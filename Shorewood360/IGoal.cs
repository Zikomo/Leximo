using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public interface IGoal
    {
        bool IsGoalCompleted
        {
            get;
        }
        TimeSpan GoalStartTime
        {
            get;
        }
        bool HasUpdated
        {
            get;
            set;
        }

        IETGames.Shorewood.GoalRenderer.GoalBox GoalBox
        {
            get;
            set;
        }

        bool CheckGoal(WordBuilder builder, bool countGoal);

        StringBuilder GoalText
        {
            get;
        }

        StringBuilder GoalTitle
        {
            get;
        }

        float GoalStatus
        {
            get;            
        }

        int Count
        {
            get;
        }

        void Activate();
        void Deactivate();



        void Reset();

    }
}
