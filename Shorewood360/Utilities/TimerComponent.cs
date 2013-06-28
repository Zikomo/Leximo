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


namespace IETGames.Shorewood.Utilities
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 

    public delegate void OnTime(GameTime gameTime, object argument);

    public struct EventTrigger:IComparable 
    {
        public TimeSpan time;
        public event OnTime eventFunction;
        public object eventArgument;
        public EventTrigger(TimeSpan time, OnTime eventFunction, object eventArgument)
        {
            this.time = time;
            this.eventFunction = eventFunction;
            this.eventArgument = eventArgument;
        }

        public void Trigger(GameTime gameTime)
        {
            if (eventFunction != null)
            {
                eventFunction(gameTime, eventArgument);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            EventTrigger comparison = (EventTrigger)obj;
            return this.time.CompareTo(comparison.time);
        }

        #endregion
    }

    public class IntervalTrigger : IComparable
    {
        public TimeSpan time;
        public TimeSpan timeSinceLastTrigger;
        public event OnTime eventFunction;
        public object eventArgument;
        public IntervalTrigger(TimeSpan time, OnTime eventFunction, object eventArgument)
        {
            this.time = time;
            this.eventFunction = eventFunction;
            this.eventArgument = eventArgument;
            this.timeSinceLastTrigger = new TimeSpan();
        }

        public void Trigger(GameTime gameTime)
        {
            if (eventFunction != null)
            {
                eventFunction(gameTime, eventArgument);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            EventTrigger comparison = (EventTrigger)obj;
            return this.time.CompareTo(comparison.time);
        }

        #endregion
    }

    public class TimerComponent : Microsoft.Xna.Framework.GameComponent
    {
        TimeSpan duration = new TimeSpan(0, 4, 0);
        
        TimeSpan time = new TimeSpan();
        bool started = false;
        List<EventTrigger> events = new List<EventTrigger>();
        List<IntervalTrigger> intervalEvents = new List<IntervalTrigger>();
        Stack<EventTrigger> eventStack = new Stack<EventTrigger>();



        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                Shorewood.timerRenderer.TimerChanged = true;
            }
        }

        public bool IsStarted
        {
            get
            {
                return started;
            }
        }

        public TimeSpan ElapsedTime
        {
            get;
            set;
        }
        
        public TimeSpan CountdownTime
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public TimerComponent(Game game)
            : base(game)
        {           
        }

        public void AddEvent(TimeSpan eventTime, OnTime specificEvent, object argument)
        {
            events.Add(new EventTrigger(eventTime, specificEvent, argument));
        }

        public void AddEvent(TimeSpan eventTime, OnTime specificEvent)
        {
            AddEvent(eventTime, specificEvent, null);
        }

        public void AddIntervalEvent(TimeSpan interval, OnTime specificEvent, object arguement)
        {
            intervalEvents.Add(new IntervalTrigger(interval, specificEvent, arguement));
        }

        public void ClearIntervalEvents()
        {
            intervalEvents.Clear();
        }


        private void LoadEvents()
        {
            events.Sort();
            eventStack.Clear();
            foreach (var trigger in events)
            {
                eventStack.Push(trigger);
            }
            foreach (var intervalTrigger in intervalEvents)
            {
                intervalTrigger.timeSinceLastTrigger = new TimeSpan(0);
            }
        }

        

        public void Start()
        {             
            started = true;
            Enabled = true;
        }

        public void Stop()
        {
            started = false;
        }

        public void Reset()
        {
            started = false;
            time = new TimeSpan();
            CountdownTime = Duration;
            ElapsedTime = new TimeSpan();
            LoadEvents();
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
            // TODO: Add your update code here
            if (started)
            {
                time -= gameTime.ElapsedGameTime;
                ElapsedTime += gameTime.ElapsedGameTime;
                //time = duration - currentTime;
                while (eventStack.Count > 0 && eventStack.Peek().time > time)
                {
                    eventStack.Pop().Trigger(gameTime);
                }
                for (int i = 0; i < intervalEvents.Count; i++)
                {
                    IntervalTrigger eventToCheck = intervalEvents[i];
                    if (eventToCheck.timeSinceLastTrigger > eventToCheck.time)
                    {
                        eventToCheck.timeSinceLastTrigger = new TimeSpan(0);
                        eventToCheck.Trigger(gameTime);
                    }
                    eventToCheck.timeSinceLastTrigger += gameTime.ElapsedGameTime;
                }
                if (time.TotalMilliseconds < 0)
                {
                    Stop();
                    Reset();
                }
            }
            base.Update(gameTime);
        }
    }
}