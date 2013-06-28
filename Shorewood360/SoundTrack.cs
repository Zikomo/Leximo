using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SoundTrack : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Dictionary<int, Song> playList = new Dictionary<int,Song>();
        public bool isShuffled = false;
        int index = 0;
        Random rand = new Random((int)DateTime.Now.Ticks);
        VisualizationData visualizationData;
        string musicBasePath = @"Audio\Music\";
        bool previousGamerHasControl = true;
        
        public SoundTrack(Game game)
            : base(game)
        {
          
            //PlaySong(playList[0]);
            MediaPlayer.IsVisualizationEnabled = true;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.ActiveSongChanged += new EventHandler(MediaPlayer_ActiveSongChanged);
            visualizationData = new VisualizationData();
            previousGamerHasControl = MediaPlayer.GameHasControl;
            
            
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {            
            if (MediaPlayer.State == MediaState.Stopped)
            {
                PlaySong(playList[NextSong()]);
            }
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        protected override void LoadContent()
        {
            playList.Add(0, Shorewood.Content.Load<Song>(musicBasePath + "Loveshadow_-_Takin_Yo_Time"));
            playList.Add(1, Shorewood.Content.Load<Song>(musicBasePath + "Loveshadow_-_Act_Cool"));
            playList.Add(2, Shorewood.Content.Load<Song>(musicBasePath + "rupert1073_-_Fiesta"));
            playList.Add(3, Shorewood.Content.Load<Song>(musicBasePath + "snowflake_-_Persephone"));
            playList.Add(4, Shorewood.Content.Load<Song>(musicBasePath + "victor_-_Ophelia_(Treatment)"));  
            base.LoadContent();
        }

        public VisualizationData GetVisualizationData()
        {
            MediaPlayer.GetVisualizationData(visualizationData);
            return visualizationData;
        }

        public void Start()
        {
            PlaySong(playList[0]);
        }

        public float MusicVolume
        {
            get
            {
                return MediaPlayer.Volume;
            }
            set
            {
                MediaPlayer.Volume = value;
                SoundEffect.MasterVolume = value + 0.2f;
            }

        }

        public bool GameHasControl
        {
            get;
            private set;
        }

        public void PlaySong(Song song)
        {
            MediaPlayer.Play(song);
            index++;
        }

        int NextSong()
        {
            //if (index == 0)
            //{
            //    PlaySong(playList[0]);
            //}
            if (isShuffled)
            {
                return rand.Next(0, playList.Count - 1);
            }
            else
            {
                int song = index  % playList.Count;
                //index++;
                return song;
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            if (!MediaPlayer.IsVisualizationEnabled)
            {
                MediaPlayer.IsVisualizationEnabled = true;
            }
            if (previousGamerHasControl != MediaPlayer.GameHasControl)
            {
                if (MediaPlayer.GameHasControl)
                {
                    PlaySong(playList[NextSong()]);
                }
            }

            GameHasControl = previousGamerHasControl = MediaPlayer.GameHasControl;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}