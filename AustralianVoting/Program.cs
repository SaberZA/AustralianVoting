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
            //var candidateCount = _candidates.Count;
            //var voteArray = new List<List<int>>
            //{
            //    new List<int> { 1,2,2,1,3 },
            //    new List<int> { 2,1,3,2,1 },
            //    new List<int> { 3,3,1,3,2 }
            //};

            //var voter = new Voter1();
            //var result = voter.CountVotes(0, voteArray);

            var voteBallots = new List<Stack<int>>
            {
                new Stack<int>(new List<int>{3,2,1}),
                new Stack<int>(new List<int>{3,1,2}),
                new Stack<int>(new List<int>{1,3,2}),
                new Stack<int>(new List<int>{3,2,1}),
                new Stack<int>(new List<int>{2,1,3})
            };

            var ballotCollection = new BallotCollection(voteBallots);
            var ballotRound = ballotCollection.GetCurrentBallotRound();
            var eliminatedCandidates = ballotRound.GetEliminatedCandidates();


            foreach(var vote in ballotRound)
            {
                Console.WriteLine(vote);
            }
            Console.WriteLine();
            foreach(var eliminatedCandidate in eliminatedCandidates)
            {
                Console.WriteLine("e: " + eliminatedCandidate);
                ballotRound.GetIndexesOfCandidate(eliminatedCandidate).ForEach(p =>
                {
                    Console.WriteLine(p);
                    ballotCollection[p].Pop();
                });                
            }

            Console.WriteLine("new ballot round");
            var newBallotRound = ballotCollection.GetCurrentBallotRound();
            foreach (var vote in newBallotRound)
            {
                Console.WriteLine(vote);
            }


            Console.ReadKey();
        }

        public class BallotCollection : List<Stack<int>>
        {
            public BallotCollection(IEnumerable<Stack<int>> voterChoices) : base(voterChoices)
            {

            }

            public BallotRound GetCurrentBallotRound()
            {
                return new BallotRound(this.Select(p => p.FirstOrDefault())
                    .ToList());
            }
        }

        public class BallotRound : List<int>
        {
            public BallotRound(IEnumerable<int> ballots) : base(ballots)
            {

            }

            public List<int> GetRemainingCandidates()
            {
                return this.Distinct().ToList();
            }

            public bool AllCandidatesTied()
            {
                return this.All(p => p == this.First());
            }

            public int GetMajorityCandidate()
            {
                return this.Max();
            }

            public bool HasMajorityCandidate()
            {
                var totalVotes = this.Sum();
                return this.Any(p => p > totalVotes / 2.00m);
            }

            public List<int> GetEliminatedCandidates()
            {
                var voteResult = GetBallotResult();

                var lowestVoteCountInBallotRound = voteResult.Min(p => p.Value);

                return voteResult.Where(p => p.Value == lowestVoteCountInBallotRound)
                    .Select(p=>p.Key)
                    .ToList();
            }

            public List<int> GetIndexesOfCandidate(int candidate)
            {
                return this.Select((p, i) =>
                {
                    if (p == candidate) return i;
                    return -1;
                })
                .Where(p => p != -1).ToList();
            }

            private Dictionary<int, int> GetBallotResult()
            {
                var candidates = this.Distinct().ToList();
                var ballotResult = new Dictionary<int, int>();

                foreach(var candidate in candidates)
                {
                    var candidateVoteCount = this.Count(p => p == candidate);
                    ballotResult.Add(candidate, candidateVoteCount);
                }

                return ballotResult;
            }
        }


















              

        public class Voter1
        {
            public List<int> CountVotes(int round, List<List<int>> voteArray)
            {
                var candidateCount = voteArray.Count;
                var currentVoteRoundArray = voteArray[round];

                if (round >= candidateCount) return new List<int>();
                if (currentVoteRoundArray.Count == 0) return new List<int>();
                if (currentVoteRoundArray.Count == 1) return new List<int> { currentVoteRoundArray.First() };

                var voteRoundResult = CountCandidates(currentVoteRoundArray, _candidates.Count);

                if (HasMajorityCandidate(voteRoundResult)) return new List<int> { GetMajorityCandidate(voteRoundResult) };
                if (AllCandidatesTied(voteRoundResult)) return GetRemainingCandidates(currentVoteRoundArray);

                return CountVotes(round + 1, RemoveLowestTied(voteArray, currentVoteRoundArray, voteRoundResult));
            }

            private List<List<int>> RemoveLowestTied(List<List<int>> voteArray, List<int> currentVoteRoundArray, List<int> voteRoundResult)
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
                        var candidateNumber = voteRoundArray[j] - 1;
                        if (_removedCandidates.Contains(candidateNumber))
                        {
                            indexesToRemove.Add(j);
                        }
                    }

                    var newVoteRoundArray = voteRoundArray.Where((p, i) => !indexesToRemove.Contains(i)).ToList();

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
                return voteRoundResult.All(p => p == voteRoundResult[0]);
            }

            private static int GetMajorityCandidate(List<int> voteRoundResult)
            {
                return voteRoundResult.Max();
            }

            private static bool HasMajorityCandidate(List<int> voteRoundResult)
            {
                var totalVotes = voteRoundResult.Sum();
                return voteRoundResult.Any(p => p > totalVotes / 2.00m);
            }

            private static List<int> CountCandidates(List<int> currentVoteRoundArray, int candidateCount)
            {
                var voteResultArray = new List<int>();

                for (var i = 0; i < candidateCount; i++)
                {
                    var candidateRoundResult = currentVoteRoundArray.Count(p => p == i + 1);
                    voteResultArray.Add(candidateRoundResult);
                }
                return voteResultArray;
            }
        }


        
    }

    
}