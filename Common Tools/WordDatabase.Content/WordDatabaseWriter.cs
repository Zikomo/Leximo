using System;
using System.Collections.Generic;
//using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = Algorithms.SuffixTree;
using Algorithms;

namespace IETGames.WordDatabase.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class WordDatabaseWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            value.BuildTree();
            SuffixTree.Save(output, value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            switch (targetPlatform)
            {
                case TargetPlatform.Windows:
                    return "IETGames.WordDatabase.Content.WordDatabaseReader, Leximo, Version=1.0.0.0, Culture=neutral";
                case TargetPlatform.Xbox360:
                    return "IETGames.WordDatabase.Content.WordDatabaseReader, Leximo, Version=1.0.0.0, Culture=neutral";
                case TargetPlatform.Zune:
                    return "IETGames.WordDatabase.Content.WordDatabaseReader, LeximoZune, Version=1.0.0.0, Culture=neutral";
                case TargetPlatform.Unknown:                
                    return "IETGames.WordDatabase.Content.WordDatabaseReader, Leximo, Version=1.0.0.0, Culture=neutral";
                default:
                    return "IETGames.WordDatabase.Content.WordDatabaseReader, Leximo, Version=1.0.0.0, Culture=neutral";
            }
        }
    }
}
