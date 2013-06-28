using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ListPool;
using System.Diagnostics;
using System.Reflection;

namespace IETGames.Shorewood
{
    class ShorewoodPool
    {
        public static Pool<Vector2> vectorPool = new Pool<Vector2>(10);
        public static Pool<WordBuilder> builderPool = new Pool<WordBuilder>(10);
        public static Pool<Letter> letterPool = new Pool<Letter>(40);
        public static Pool<FloatingPoints> floatingPointsPool = new Pool<FloatingPoints>(40);
        public static Pool<StringBuilder> stringBuilderPool = new Pool<StringBuilder>(10);
        //public static Pool<LightningBolt> lightningBoltPool = new Pool<LightningBolt>(10);
        public static int boltAllowedToAllocate = 10;
        public static int boltsAllocated = 0;
        public static int buildersAllocated = 0;
        public static int floatingPointsAllocated = 0;

        public static void InitalizePools()
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    ReturnBuilder((new WordBuilder(1)));
            //}
        }

        public static void ClearPools()
        {
            builderPool.ClearLists();
            builderPool.ClearObjects();
            floatingPointsPool.ClearLists();
            floatingPointsPool.ClearObjects();
            stringBuilderPool.ClearLists();
            stringBuilderPool.ClearObjects();
        }


        //public static LightningBolt GetBolt(Game game, Vector3 source, Vector3 destination, LightningDescriptor descriptor)
        //{
        //    LightningBolt bolt = lightningBoltPool.Get();
        //    if (bolt == null)
        //    {
        //        if (boltsAllocated < boltAllowedToAllocate)
        //        {
        //            bolt = new LightningBolt(game, source, destination, descriptor);
        //            boltsAllocated++;
        //        }
        //    }
        //    else
        //    {
        //        bolt.LightningDescriptor = descriptor;
        //        bolt.Destination = destination;
        //        bolt.Source = source;
        //    }
        //    return bolt;
        //}


        public static StringBuilder GetStringBuilder(StringBuilder builder)
        {
            StringBuilder newBuilder = stringBuilderPool.Get();

            if (newBuilder == null)
            {
                newBuilder = new StringBuilder(builder.ToString());
            }
            else
            {
                newBuilder.Remove(0, newBuilder.Length);
                newBuilder.Insert(0, builder.ToString());
            }
            return newBuilder;
        }

        

//        public static void ReturnBuilder(WordBuilder builder)
//        {
//            if (builder.isDisposed)
//            {
//                throw new ObjectDisposedException("Word Builder");
//            }
//#if !XBOX
//            //StackTrace tracer = new StackTrace();
//            //StackFrame frame = tracer.GetFrame(1);
//            //MethodBase function  = frame.GetMethod();
//            //builder.caller = function.Name;
//#endif 
//            //ShorewoodPool.letterPool.ReturnList(builder.letters);
//            builder.isDisposed = true;
//            ShorewoodPool.builderPool.Return(builder);
//            buildersAllocated--;
//            //Console.WriteLine(buildersAllocated);
//        }

        //public static WordBuilder GetBuilder(int gridWidth)
        //{
        //    WordBuilder newBuilder = builderPool.Get();
        //    if ((object)newBuilder == null)
        //    {
        //        newBuilder = new WordBuilder(gridWidth);
        //    }
        //    else
        //    {
        //        newBuilder.Clear();
        //    }
        //    newBuilder.isDisposed = false;
        //    newBuilder.isValidWord = false;
        //    buildersAllocated++;
        //    return newBuilder;
        //}

        //public static WordBuilder GetBuilder(WordBuilder builder)
        //{
        //    WordBuilder newBuilder = builderPool.Get();            
        //    if ((object)newBuilder == null)
        //    {
        //        newBuilder = new WordBuilder(builder);
        //        Console.WriteLine("Allocating WordBuilder: " + buildersAllocated);                
        //    }
        //    else
        //    {
        //        newBuilder.Copy(builder);
        //    }
            
        //    newBuilder.isDisposed = false;
        //    newBuilder.isValidWord = false;
        //    buildersAllocated++;
            
        //    return newBuilder;
        //}

        

        //public static Vector2 GetVector(float x, float y)
        //{
        //    Vector2 rtnValue = vectorPool.Get();
        //    rtnValue.X = x;
        //    rtnValue.Y = y;
        //    return rtnValue;
        //}
    }
}