using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.IO;

namespace IETGames.Shorewood.Storage
{
    public class GamerEntry:IEquatable<GamerEntry>
    {
        bool changed = false;
        Texture2D gamerPicture;
        [XmlIgnore]
        public Texture2D GamerPicture
        {
            get
            {
                if (changed)
                {
                    gamerPicture = new Texture2D(Shorewood.graphics.GraphicsDevice, 64, 64);
                    gamerPicture.SetData<uint>(gamerPicturePixels);
                    changed = false;
                }
                return gamerPicture;
            }
            set
            {
                gamerPicture = value;                
                gamerPicturePixels = new uint[gamerPicture.Width * gamerPicture.Height];
                this.gamerPicture.GetData<uint>(gamerPicturePixels);
                changed = true;
            }
        }
        [XmlIgnore]
        public uint[] gamerPicturePixels;
        public string gamerTag = "";
        public GamerEntry()
        {
            gamerPicturePixels = new uint[Shorewood.peoplePixels.Length];            
            Shorewood.peoplePixels.CopyTo(gamerPicturePixels,0);
            changed = true;
        }

        static public void WriteEntry(BinaryWriter stream, GamerEntry entry)
        {
            stream.Write(entry.gamerTag);
            for (int i = 0; i < entry.gamerPicturePixels.Length; i++)
            {
                stream.Write(entry.gamerPicturePixels[i]);
            }
        }

        static public GamerEntry ReadEntry(BinaryReader stream)
        {
            GamerEntry rtnEntry = new GamerEntry();
            rtnEntry.gamerTag = stream.ReadString();
            for (int i = 0; i < rtnEntry.gamerPicturePixels.Length; i++)
            {
                rtnEntry.gamerPicturePixels[i] = stream.ReadUInt32();
            }
            return rtnEntry;
        }

        #region IEquatable<GamerEntry> Members

        public bool Equals(GamerEntry other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return other.gamerTag == this.gamerTag;
        }

        #endregion
    }
}