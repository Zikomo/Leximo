using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood.Storage
{
    public class ScoreEntry:IEquatable<ScoreEntry>, IComparable<ScoreEntry>
    {
        string gamerTag;
        public ScoreEntry()
        {
        }

        public string GamerTag
        {
            get
            {
                return gamerTag;
            }
            set
            {
                gamerTag = value;
            }
        }

        public int Score
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public Difficulty Difficulty
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            int result = Score;
            result = (result * 397) ^ (int)Duration.TotalMinutes;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (obj.GetType() != typeof(ScoreEntry)) return false;
            return base.Equals((ScoreEntry)obj);
        }

        #region IEquatable<ScoreEntry> Members

        public bool Equals(ScoreEntry other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return (this.Score == other.Score) && (this.Difficulty == other.Difficulty) && (this.Duration == other.Duration) && (this.GamerTag == other.GamerTag);
        }

        #endregion

        #region IComparable<ScoreEntry> Members

        public int CompareTo(ScoreEntry other)
        {
            return other.Score - this.Score;
        }

        #endregion
    }
}