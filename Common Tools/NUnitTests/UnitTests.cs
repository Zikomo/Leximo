using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NUnit.Framework;
using Algorithms;
using System.IO;


namespace NUnitTests
{
    [TestFixture]
    public class UnitTests
    {
        protected string theString ="";
        protected List<string> individualStrings = new List<string>();

        [SetUp]
        public void SetUp()
        {
            
            using (StreamReader reader = new StreamReader(@"..\..\words\2of4brif.txt"))
            {
                theString = reader.ReadToEnd();
                string[] pattern = {"\r\n"};
                string[] splits = theString.Split(pattern, StringSplitOptions.RemoveEmptyEntries);
                foreach(String individualString in splits)
                {
                    individualStrings.Add(individualString);
                    //Console.WriteLine("Adding: " + individualString);
                }
            }
        }
        [Test]
        public void VerifyValidWordsFromFile()
        {
            SuffixTree tree = new SuffixTree(theString);
            tree.BuildTree();
            using (FileStream writeFile = new FileStream("suffixtreetest", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                SuffixTree.Save(writeFile, tree);
            }

            SuffixTree testTree;

            using (FileStream readFile = new FileStream("suffixtreetest", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                testTree = SuffixTree.LoadFromFile(readFile);
            }
            foreach (string individualString in individualStrings)
            {
                Assert.IsTrue(testTree.Search(individualString));
            }
        }

        //Make sure all of the words can be found in the tree
        [Test]
        public void VerifyValidWords()
        {
            SuffixTree tree = new SuffixTree(theString);
            tree.BuildTree();
            foreach (string individualString in individualStrings)
            {
                Assert.IsTrue(tree.Search(individualString));
            }
        }
        //Throw random garbage at the tree and see if the search returns false positives
        [Test]
        public void VerifyRandomInvalidWords()
        {
            SuffixTree tree = new SuffixTree(theString);
            tree.BuildTree();

            Random random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < individualStrings.Count; i++)
            {
                StringBuilder builder = new StringBuilder();
                for (int j = 5; j < random.Next(15); j++)
                {
                    builder.Append(random.Next('a', 'z'));
                }
                string builtString = builder.ToString();
                string message = "Random String " + builtString + " was found!";

                //I originally checked to see if builder is in individualStrings, however with such a large 
                //data set it took way too long to execute. There is a risk that a random string of 5 to 15
                //characters IS in the word list!
                Assert.IsTrue(!tree.Search(builtString));
            }
        }

        [Test]
        public void VerifyPartialRandomInvalidWords()
        {
            SuffixTree tree = new SuffixTree(theString);
            tree.BuildTree();

            Random random = new Random((int)DateTime.Now.Ticks);
            foreach (string individualString in individualStrings)
            {
                StringBuilder builder = new StringBuilder(individualString);
                //this will inject random characters into valid words
                for (int j = random.Next(individualString.Length-2); j < random.Next(individualString.Length); j++)
                {                   
                    builder.Insert(j,random.Next('a', 'z'));                    
                }
                string builtString = builder.ToString();
                string message = "Corrupting: " + individualString + " as " + builtString;
                //I originally checked to see if builder is in individualStrings, however with such a large 
                //data set it took way too long to execute. There is a risk that a random string of 5 to 15
                //characters IS in the word list!
                if (!individualStrings.Contains(builtString))
                {
                    Assert.IsTrue(!tree.Search(builtString), message);
                }
            }
        }
    }
}
