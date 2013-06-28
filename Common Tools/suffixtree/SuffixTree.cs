/* SuffixTree.cs
 * To Do: add comments
 * 
 * 
 * This is a suffix tree algorithm for .NET written in C#. Feel free to use it as you please!
 * This code was derived from Mark Nelson's article located here: http://marknelson.us/1996/08/01/suffix-trees/
 * Have Fun 
 * 
 * Zikomo A. Fields 2008
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algorithms
{

    public class SuffixTree
    {        
        public static string theString;       
        public static Dictionary<int, Edge> Edges = null;
        public static Dictionary<int, Node> Nodes = null;
        public static StringBuilder[] FourLetterWords;
        public static StringBuilder[] FiveLetterWords;
        public static StringBuilder[] SixLetterWords;
        public static StringBuilder[] SubStrings;
        public SuffixTree(string theString)
        {
            SuffixTree.theString = theString;
            Nodes = new Dictionary<int, Node>();
            Edges = new Dictionary<int, Edge>();            
        }

        public void BuildTree()
        {
            Suffix active = new Suffix(SuffixTree.theString, Edges, 0, 0, -1);
            for (int i = 0; i <= theString.Length - 1; i++)
            {
                AddPrefix(active, i);
            }
        }

        public static void WriteWordArrays(BinaryWriter writer, string[] words, List<string>subStrings)
        {
            writer.Write(words.Length);
            foreach (var word in words)
            {
                writer.Write(word);
                if (subStrings != null)
                {
                    if (!subStrings.Contains(word))
                    {
                        subStrings.Add(word.Substring(1, 3));
                    }
                }
            }
        }

        public static void WriteFixedLengthWords(BinaryWriter writer)
        {
            string[] splitter = { "\r\n" };
            string[] words = SuffixTree.theString.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            List<string> fourLetterWords = new List<string>();
            List<string> fiveLetterWords = new List<string>();
            List<string> sixLetterWords = new List<string>();
            List<string> subStrings = new List<string>();
            foreach (var word in words)
            {
                if (word.Length == 4)
                {
                    fourLetterWords.Add(word);
                }
                else if (word.Length == 5)
                {
                    fiveLetterWords.Add(word);
                }
                else if (word.Length == 6)
                {
                    sixLetterWords.Add(word);
                }
            }

            WriteWordArrays(writer, fourLetterWords.ToArray<string>(), null);
            WriteWordArrays(writer, fiveLetterWords.ToArray<string>(), subStrings);
            WriteWordArrays(writer, sixLetterWords.ToArray<string>(), subStrings);
            WriteWordArrays(writer, subStrings.ToArray<string>(), null);
        }
        public static void Save(BinaryWriter writer, SuffixTree tree)
        {
            writer.Write(SuffixTree.Edges.Count);
            writer.Write(SuffixTree.theString.Length);
            writer.Write(SuffixTree.theString);
            foreach (KeyValuePair<int, Edge> edgePair in SuffixTree.Edges)
            {
                writer.Write(edgePair.Key);
                writer.Write(edgePair.Value.endNode);
                writer.Write(edgePair.Value.startNode);
                writer.Write(edgePair.Value.indexOfFirstCharacter);
                writer.Write(edgePair.Value.indexOfLastCharacter);
            }
            WriteFixedLengthWords(writer);
        }


        public static void Save(Stream stream, SuffixTree tree)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Save(writer, tree);
            }
        }

        public static StringBuilder[] ReadFixedLengthWords(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            StringBuilder[] words = new StringBuilder[count];
            for (int i = 0; i < count; i++)
            {
                words[i] = new StringBuilder(reader.ReadString());
            }
            return words;
        }

        public static SuffixTree LoadFromFile(BinaryReader reader)
        {
            SuffixTree tree;
            int count = reader.ReadInt32();
            int theStringLength = reader.ReadInt32();
            string theString = reader.ReadString();
            tree = new SuffixTree(theString);
            for (int i = 0; i < count; i++)
            {
                int key = reader.ReadInt32();
                Edge readEdge = new Edge(-1);
                readEdge.endNode = reader.ReadInt32();
                readEdge.startNode = reader.ReadInt32();
                readEdge.indexOfFirstCharacter = reader.ReadInt32();
                readEdge.indexOfLastCharacter = reader.ReadInt32();
                SuffixTree.Edges.Add(key, readEdge);
            }
            SuffixTree.FourLetterWords = ReadFixedLengthWords(reader);
            SuffixTree.FiveLetterWords = ReadFixedLengthWords(reader);
            SuffixTree.SixLetterWords = ReadFixedLengthWords(reader);
            SuffixTree.SubStrings = ReadFixedLengthWords(reader);
            return tree;
        }

        public static SuffixTree LoadFromFile(Stream stream)
        {
            SuffixTree tree;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                tree = LoadFromFile(reader);
            }
            return tree;
        }

        public char Peek(StringBuilder search)
        {
            //search = search.ToLower();
            if (search.Length == 0)
            {
                return '0';
            }
            int index = 0;
            Edge edge;
            if (!SuffixTree.Edges.TryGetValue((int)Edge.Hash(0, search[0]), out edge))
            {
                return '0';
            }

            if (edge.startNode == -1)
            {
                return '0';
            }
            else
            {
                for (; ; )
                {
                    for (int j = edge.indexOfFirstCharacter; j <= edge.indexOfLastCharacter; j++)
                    {
                        if (index >= search.Length)
                        {
                            if (index < theString.Length)
                            {
                                return theString[index+1];
                            }
                            else
                            {
                                return '0';
                            }

                        }
                        char test = theString[j];
                        if (SuffixTree.theString[j] != search[index++])
                        {
                            return '0';
                        }
                    }
                    if (index < search.Length)
                    {
                        Edge value;
                        if (SuffixTree.Edges.TryGetValue(Edge.Hash(edge.endNode, search[index]), out value))
                        {
                            edge = value;
                        }
                        else
                        {
                            return '0';
                        }
                    }
                    else
                    {
                        if (index < theString.Length)
                        {
                            return theString[index+1];
                        }
                        else
                        {
                            return '0';
                        }
                    }
                }
            }


        }

        public bool Search(StringBuilder search)
        {           
            //search = search.ToLower();
            if (search.Length == 0)
            {
                return false;
            }
            int index = 0;
            Edge edge;
            if (!SuffixTree.Edges.TryGetValue((int)Edge.Hash(0, search[0]), out edge))
            {
                return false;
            }                

            if (edge.startNode == -1)
            {
                return false;
            }
            else
            {
                for (; ; )
                {
                    for (int j = edge.indexOfFirstCharacter; j <= edge.indexOfLastCharacter; j++)
                    {
                        if (index >= search.Length)
                        {
                            return true;
                        }
                        char test = theString[j];
                        if (SuffixTree.theString[j] != search[index++])
                        {
                            return false;
                        }
                    }
                    if (index < search.Length)
                    {
                        Edge value;
                        if (SuffixTree.Edges.TryGetValue(Edge.Hash(edge.endNode, search[index]), out value))
                        {
                            edge = value;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }



        private void AddPrefix(Suffix active, int indexOfLastCharacter)
        {
            int parentNode;
            int lastParentNode = -1;

            for (; ; )
            {
                Edge edge = new Edge(-1);
                parentNode = active.originNode;

                if (active.IsExplicit)
                {
                    edge = Edge.Find(SuffixTree.theString, SuffixTree.Edges, active.originNode, theString[indexOfLastCharacter]);
                    if (edge.startNode != -1)
                    {
                        break;
                    }
                }
                else
                {
                    edge = Edge.Find(SuffixTree.theString, SuffixTree.Edges, active.originNode, theString[active.indexOfFirstCharacter]);
                    int span = active.indexOfLastCharacter - active.indexOfFirstCharacter;
                    if (theString[edge.indexOfFirstCharacter + span + 1] == theString[indexOfLastCharacter])
                    {
                        break;
                    }
                    parentNode = Edge.SplitEdge(active, theString, Edges, Nodes,ref edge);
                }

                Edge newEdge = new Edge(SuffixTree.theString, indexOfLastCharacter, SuffixTree.theString.Length - 1, parentNode);                
                Edge.Insert(newEdge);
                if (lastParentNode > 0)
                {
                    Nodes[lastParentNode].suffixNode = parentNode;                   
                }
                lastParentNode = parentNode;

                if (active.originNode == 0)
                {
                    active.indexOfFirstCharacter++;
                }
                else
                {
                    active.originNode = Nodes[active.originNode].suffixNode;
                }                
                active.Canonize();
            }
            if (lastParentNode > 0)
            {
                Nodes[lastParentNode].suffixNode = parentNode;
            }
            active.indexOfLastCharacter++;
            active.Canonize();
        }
    }
}
