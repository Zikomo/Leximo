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
    public struct Letter
    {
        public Texture2D texture;
        public Texture2D texture1337;
        public Vector2 position;
        public Vector2 startPosition;
        public float rotation;
        public Vector2 center;
        public char letter;
        public Rectangle region;
        public float scale;
        public float startScale;
        public Vector2 gridPosition;
        public Vector2 destination;
        public float width;
        public float height;
        public bool isVisible;
        public bool changed;
        public Color color;
        public bool isGlowing;

        public Vector2  rightNeighbor;
        public Vector2 upperRightNeighbor;
        public Vector2 lowerRightNeighbor;
        public Vector2 leftNeighbor;
        public Vector2 upperLeftNeighbor;
        public Vector2 lowerLeftNeighbor;
        public bool isPartOfAValidWord;
        
        public Letter(Texture2D loadedTexture, Texture2D texture1337, char letter)
        {
            this.rotation = 0.0f;
            this.position = new Vector2(0, 0);
            this.startPosition = this.position;
            this.texture = loadedTexture;
            this.gridPosition = new Vector2(0, 0);
            this.width = 100;
            this.height =150;
            this.texture1337 = texture1337;
            if (this.texture != null)
            {
                width = texture.Width;
                height = texture.Height;
            }

            this.center = new Vector2(width / 2, 0);
            this.region = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
            this.letter = letter;
            this.upperRightNeighbor = new Vector2(-1, -1);
            this.lowerRightNeighbor = new Vector2(-1, -1);
            this.rightNeighbor = new Vector2(-1, -1);
            this.upperLeftNeighbor = new Vector2(-1, -1);
            this.lowerLeftNeighbor = new Vector2(-1, -1);
            this.leftNeighbor = new Vector2(-1, -1);
            this.color = Color.White;
            //this.color.A = 128;
            this.isVisible = true;
            this.scale = 1;
            this.startScale = this.scale;
            this.destination = this.startPosition;
            this.isPartOfAValidWord = false;
            this.changed = false;
            this.isGlowing = false;
            
        }

        //public override string ToString()
        //{
        //    return letter + "@" + gridPosition.ToString();
        //}


        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Letter))
            {
                Letter rhs = (Letter)obj;
                return this.letter == rhs.letter;
            }
            return false;
        }

        public static bool operator ==(Letter lhs, Letter rhs)
        {
            return lhs.letter == rhs.letter;
        }

        public static bool operator !=(Letter lhs, Letter rhs)
        {
            return lhs.letter != rhs.letter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Letter EmptyLetter
        {
            get
            {
                return new Letter(null, null, ' ') ;
            }
        }
    }
}