using System;
using System.Collections.Generic;
using System.Linq;

namespace AustralianVoting
{
    internal class Program
    {
        private static List<int> _removedCandidates = new List<int>();
        private static List<string> _candidates = new List<string> {"c1","c2","c3"};
        
        public static void Main(string[] args)
        {
            // have to adjust the last ballot forward since 3 can no longer be listed first!
            var candidateCount = _candidates.Count;
            var voteArray = new List<List<int>>
            {
                new List<int> { 1,2,2,1,3 },
                new List<int> { 2,1,3,2,1 },
                new List<int> { 3,3,1,3,2 }
            };
            

            var result = CountVotes(0, voteArray);

            foreach (var i in result)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }

        private static List<int> CountVotes(int round, List<List<int>> voteArray)
        {
            var candidateCount = voteArray.Count;
            var currentVoteRoundArray = voteArray[round];

            if (round >= candidateCount) return new List<int>();
            if (currentVoteRoundArray.Count == 0) return new List<int>();
            if (currentVoteRoundArray.Count == 1) return new List<int> {currentVoteRoundArray.First()};

            var voteRoundResult = CountCandidates(currentVoteRoundArray, _candidates.Count);

            if (HasMajorityCandidate(voteRoundResult)) return new List<int> {GetMajorityCandidate(voteRoundResult)};
            if (AllCandidatesTied(voteRoundResult)) return GetRemainingCandidates(currentVoteRoundArray);

            return CountVotes(round + 1, RemoveLowestTied(voteArray, currentVoteRoundArray, voteRoundResult));
        }

        private static List<List<int>> RemoveLowestTied(List<List<int>> voteArray, List<int> currentVoteRoundArray, List<int> voteRoundResult)
        {
            var lowestVoteCandidate = voteRoundResult.Min();
            var lowestCandidateIndexes = new List<int>();
            for (int i = 0; i < _candidates.Count; i++)
            {
                var candidateNumber = i + 1;
                var candidateVoteCount = currentVoteRoundArray.Count(p => candidateNumber == p);
                if (candidateVoteCount == lowestVoteCandidate)
                {
                    lowestCandidateIndexes.Add(i);
                }
            }
            
//            var lowestCandidateIndexes = voteRoundResult.Where((n, i) => currentVoteRoundArray.Count(p=>p == n) == lowestVoteCandidate)
//                .Select((n,i) => i).ToList();

            foreach (var lowestCandidateIndex in lowestCandidateIndexes)
            {
                var candidate = currentVoteRoundArray[lowestCandidateIndex];
                _removedCandidates.Add(candidate);
            }

            var newVoteArray = new List<List<int>>();
            foreach (var voteRoundArray in voteArray)
            {
                var indexesToRemove = new List<int>();
                for (int j = 0; j < voteRoundArray.Count; j++)
                {
                    // remove from _removedCandidates
                    var candidateNumber = voteRoundArray[j] - 1;
                    if (_removedCandidates.Contains(candidateNumber))
                    {
                        indexesToRemove.Add(j);
                    }
                }
//                indexesToRemove.ForEach(p=> voteRoundArray.RemoveAt(p));

                var newVoteRoundArray = voteRoundArray.Where((p, i) => !indexesToRemove.Contains(i)).ToList();
                
//                foreach (var i in indexesToRemove)
//                {
//                    voteRoundArray.RemoveAt(i);
//                    
//                }
                newVoteArray.Add(newVoteRoundArray);
            }
            
            _removedCandidates.ForEach(p =>
            {
                if (p >= 0 && p < _candidates.Count)
                {
                    _candidates.RemoveAt(p);
                }
            });

            return newVoteArray;
        }

        private static List<int> GetRemainingCandidates(List<int> currentVoteRoundArray)
        {
            return currentVoteRoundArray.Distinct().ToList();
        }

        private static bool AllCandidatesTied(List<int> voteRoundResult)
        {
            return voteRoundResult.All(p=>p == voteRoundResult[0]);
        }

        private static int GetMajorityCandidate(List<int> voteRoundResult)
        {
            return voteRoundResult.Max();
        }

        private static bool HasMajorityCandidate(List<int> voteRoundResult)
        {
            var totalVotes = voteRoundResult.Sum();
            return voteRoundResult.Any(p=> p > totalVotes/2.00m);
        }

        private static List<int> CountCandidates(List<int> currentVoteRoundArray, int candidateCount)
        {
            var voteResultArray = new List<int>();
            
            for (var i = 0; i < candidateCount; i++)
            {
                var candidateRoundResult = currentVoteRoundArray.Count(p => p == i + 1);
//                voteResultArray[i] = candidateRoundResult;
                voteResultArray.Add(candidateRoundResult);
            }
            return voteResultArray;
        }
    }
}