using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace IETGames.Shorewood.Storage
{
    public class Settings
    {
        //private float musicVolume = 0.75f;
        private float sfxVolume = 0.5f;
        public Settings()
        {
            MusicVolume = 0.25f;
        }

        public float MusicVolume
        {
            get
            {
                return Shorewood.soundTrack.MusicVolume;
            }
            set
            {
                Shorewood.soundTrack.MusicVolume = value;  
                
            }
        }

        public float SfxVolume
        {
            get
            {
                return sfxVolume;
            }
            set
            {
                sfxVolume = value;
            }
        }
    }
}