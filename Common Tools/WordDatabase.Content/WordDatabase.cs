using System;
using System.Collections.Generic;
using Algorithms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;


using TImport = Algorithms.SuffixTree;
using System.IO;

namespace IETGames.WordDatabase.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".txt", DisplayName = "Word Database", DefaultProcessor = "WordDatabaseProcessor")]
    public class WordDatabase : ContentImporter<TImport>
    {

        public override TImport Import(string filename, ContentImporterContext context)
        {
            string words = System.IO.File.ReadAllText(filename);
            return new TImport(words);
            //using (FileStream wordDB = new FileStream(filename, FileMode.Open))
            //{
            //    using (StreamReader reader = new StreamReader(wordDB))
            //    {
            //        string words = reader.ReadToEnd();
            //        return new TImport(words);
            //    }
            //}            
        }
    }
}
