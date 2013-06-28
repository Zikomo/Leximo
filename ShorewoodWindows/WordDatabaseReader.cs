using System;
using System.Collections.Generic;
using Algorithms;
using System.Text;
using Microsoft.Xna.Framework.Content;

using TRead = Algorithms.SuffixTree;

namespace IETGames.WordDatabase.Content
{
    public class WordDatabaseReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {            
            return SuffixTree.LoadFromFile(input);            
        }
    }
}
