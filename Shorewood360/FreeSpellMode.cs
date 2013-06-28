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
using IETGames.Shorewood.Avatars;



namespace IETGames.Shorewood
{

    public class AnimationHelper
    {
        public int Start = 0;
        
        public int AnimationPosition = 0;
        public int Length = 10;
        public Vector2 Position = new Vector2(0, 720 - 256);

        
        TimeSpan increment = new TimeSpan(0, 0, 0, 0, 33);
        TimeSpan time = TimeSpan.Zero;

        public AnimationHelper()
        {

        }

        public AnimationHelper(int start, int length, int startPosition)
        {
            Start = start;            
            Length = length;
            AnimationPosition = startPosition;
            AnimationPosition = AnimationPosition % Length;
        }

        public void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime;
            if (time > increment)
            {
                AnimationPosition++;
                time = TimeSpan.Zero;
            }
            AnimationPosition = AnimationPosition % Length;
            if (AnimationPosition == 0)
            {
                AnimationPosition = 1;
            }
        }

    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FreeSpellMode : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //AvatarHandler avatar = new AvatarHandler();
        AnimationHelper[] crowd = new AnimationHelper[10];       
        TimeSpan time = TimeSpan.Zero;
        TimeSpan phaseAnimationDuration = new TimeSpan(0, 0, 1);
        TimeSpan phaseTime = TimeSpan.Zero;
        Vector2 phaseInAnimationDestination = new Vector2(0, 720 - 256);
        Vector2 phaseOutAnimationDestination = new Vector2(0, 720);
        Vector2 phaseStartPosition = Vector2.Zero;
        Vector2 phaseDestination = Vector2.Zero;
        Vector2 phasePosition = Vector2.Zero;
        public bool IsPhasing = false;
        public bool IsPhasingOut = false;
        bool IsLoaded = false;
        
        public FreeSpellMode(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Visible = false;
            Enabled = true;

        }

        public void LoadAnimation()
        {
            for (int i = 0; i < crowd.Length; i++)
            {
                int crowdType = Shorewood.Random.Next(0, 3);
                crowd[i] = new AnimationHelper();
                switch (crowdType)
                {
                    case 0:
                        crowd[i].Start = 0;
                        break;
                    case 1:
                        crowd[i].Start = 1;
                        break;
                    case 2:
                        crowd[i].Start = 2;
                        break;
                }
                crowd[i].AnimationPosition = Shorewood.Random.Next(0, Shorewood.avatar2DAnimation.AvatarAnimation[crowd[i].Start].Count);
                crowd[i].Length = Shorewood.avatar2DAnimation.AvatarAnimation[crowd[i].Start].Count-1;
                if (i < 5)
                {
                    crowd[i].Position = new Vector2(i*450/crowd.Length, 0);
                }
                else
                {
                    crowd[i].Position = new Vector2((i-5) * 450 / crowd.Length + 800, 0);
                }
            }
            IsLoaded = true;
        }

        public void Show()
        {
            if (!Visible && Shorewood.Difficulty != Difficulty.FreeSpell || Shorewood.gameState == GameState.Credits)
            {
                Visible = true;
                phaseTime = TimeSpan.Zero;
                phasePosition = phaseStartPosition = phaseOutAnimationDestination;
                phaseDestination = phaseInAnimationDestination;
                IsPhasing = true;
                IsPhasingOut = false;
                Shorewood.cheer.Play();
                
                //Enabled = true;
            }
        }

        public void Hide()
        {
            if (Visible & !IsPhasingOut)
            {                
                phaseTime = TimeSpan.Zero;
                phaseStartPosition = phaseInAnimationDestination;
                phaseDestination = phaseOutAnimationDestination;
                IsPhasing = true;
                IsPhasingOut = true;
            }
        }

        protected override void LoadContent()
        {

            
            base.LoadContent();
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            if (Visible)
            {
               // avatar.Start();
                time = TimeSpan.Zero;
            }
            else
            {
               // avatar.Stop();
            }
            base.OnVisibleChanged(sender, args);
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {  

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //avatar.Update(gameTime);
            if (IsLoaded&&Visible)
            {
                for (int i = 0; i < crowd.Length; i++)
                {
                    crowd[i].Update(gameTime);
                }
                if (IsPhasing)
                {
                    phasePosition = Vector2.SmoothStep(phaseStartPosition, phaseDestination, (float)phaseTime.TotalMilliseconds / (float)phaseAnimationDuration.TotalMilliseconds);
                    phaseTime += gameTime.ElapsedGameTime;
                    if (phaseTime > phaseAnimationDuration)
                    {
                        IsPhasing = false;
                        if (phaseDestination == phaseOutAnimationDestination)
                        {
                            Visible = false;
                        }
                    }
                }

            }
            
            base.Update(gameTime);
        }

        protected void DrawCrowd(GameTime gameTime)
        {
#if XBOX
            for (int i = 0; i < crowd.Length; i++)
            {
                Shorewood.spriteBatch.Draw(Shorewood.avatar2DAnimation.AvatarAnimation[crowd[i].Start][crowd[i].AnimationPosition], crowd[i].Position + phasePosition, Color.Black);
            }
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            DrawCrowd(gameTime);
            Shorewood.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}