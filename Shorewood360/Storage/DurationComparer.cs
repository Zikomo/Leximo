using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood.Storage
{
    public class DurationComparer:IComparer<ScoreEntry>
    {
        public DurationComparer()
        {
        }

        #region IComparer<ScoreEntry> Members
        public int Compare(ScoreEntry x, ScoreEntry y)
        {
            return (int)(x.Duration.TotalSeconds - y.Duration.TotalSeconds);
        }
        #endregion
    }
}