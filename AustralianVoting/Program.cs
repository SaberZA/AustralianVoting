using System;
using System.Collections.Generic;

namespace AustralianVoting
{
    internal partial class Program
    {
        private static List<int> _removedCandidates = new List<int>();
        private static List<string> _candidates = new List<string> {"c1","c2","c3"};
        
        public static void Main(string[] args)
        {
            var voteBallots = new List<Stack<int>>
            {
                new Stack<int>(new List<int>{3,2,1}),
                new Stack<int>(new List<int>{3,1,2}),
                new Stack<int>(new List<int>{1,3,2}),
                new Stack<int>(new List<int>{3,2,1}),
                new Stack<int>(new List<int>{2,1,3})
            };

            var ballotCollection = new BallotCollection(voteBallots);

            var result = ballotCollection.CountAllVotes();
            foreach(var winner in result)
            {
                Console.WriteLine(_candidates[winner-1]);
            }
            
            Console.ReadKey();
        }
    }    
}