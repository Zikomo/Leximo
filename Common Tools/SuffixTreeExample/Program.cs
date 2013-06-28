/* Program.cs
 * To Do: add comments
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
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a string: ");
            string something = Console.ReadLine();            
            SuffixTree tree = new SuffixTree(something);
            tree.BuildTree();

            while (true)
            {
                Console.Write("Search for: ");
                string searchTerm = Console.ReadLine();
                if (tree.Search(searchTerm))
                {
                    Console.WriteLine("It's in there!");
                }
                else
                {
                    Console.WriteLine("Nope not in there!");
                }
                if (searchTerm.ToLower() == "quit")
                {
                    break;
                }
            }            
        }
    }
}
