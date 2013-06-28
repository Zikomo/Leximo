using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood
{
    public struct FloatingPoints
    {
        public Color color;
        public Vector2 position;
        public Vector2 destination;
        public Vector2 startPosition;
        public float scale;
        public float endScale;
        public int points;
        //public TimeSpan startTime;
        public TimeSpan timeAlive;
        public TimeSpan duration;
        public StringBuilder text;
        public float rotation;
        public bool isAlive;
        public Vector2 origin;

        public FloatingPoints(Vector2 startPosition, Vector2 destination, int points)
        {
            text = new StringBuilder(100);
            color = Color.ForestGreen;
            position = startPosition;
            this.destination = destination;
            this.startPosition = startPosition;
            scale = 0.5f;
            endScale = 2;
            this.points = points;
            //startTime = TimeSpan.Zero;
            timeAlive = TimeSpan.Zero;
            duration = new TimeSpan(0,0,0,0,2000);
            text.Append('+');
            text.Append(points);
            text.Append('!');
            //text = "+" + this.points.ToString() + "!";
            rotation = 0;
            isAlive = true;
            origin = Vector2.Zero;
        }

        public FloatingPoints(Vector2 startPosition, Vector2 destination, StringBuilder text)
        {
            text = new StringBuilder(100);
            color = Color.ForestGreen;
            position = startPosition;
            this.destination = destination;
            this.startPosition = startPosition;
            scale = 0.5f;
            endScale = 2;
            this.points = 0;
            //startTime = TimeSpan.Zero;
            timeAlive = TimeSpan.Zero;
            duration = new TimeSpan(0, 0, 0, 0, 2000); ;
            this.text = text;            
            rotation = 0;
            isAlive = true;
            origin = Vector2.Zero;
        }

        public void Reset()
        {
            color = Color.Blue;
            position = Vector2.Zero;
            scale = 1;
            endScale = 3;
            points = 0;
            //startTime = TimeSpan.Zero;
            timeAlive = TimeSpan.Zero;
            duration = new TimeSpan(0, 0, 0, 0, 2000);
            text.Remove(0, text.Length);
            rotation = 0;
            isAlive = true;
        }

        public void Update(GameTime time)
        {
            timeAlive += time.ElapsedGameTime;
            //double currentTime = time.TotalGameTime.TotalMilliseconds;
            //timeAlive = currentTime - startTime;
            float step = (float)(timeAlive.TotalMilliseconds / duration.TotalMilliseconds);
            scale = (float)step * endScale;
            isAlive = timeAlive < duration;
            position = Vector2.SmoothStep(startPosition, destination + new Vector2(Shorewood.titleSafeArea.X, Shorewood.titleSafeArea.Y), step);
            
        }
    }
}