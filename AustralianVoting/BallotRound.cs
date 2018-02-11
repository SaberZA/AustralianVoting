using System.Collections.Generic;
using System.Linq;

namespace AustralianVoting
{
    internal partial class Program
    {
        public class BallotRound : List<int>
        {
            private readonly BallotCollection _parentBallotCollection;

            public BallotRound(IEnumerable<int> ballots, BallotCollection parentBallotCollection) : base(ballots)
            {
                _parentBallotCollection = parentBallotCollection;
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
                var totalVotes = this.Count();
                var voteResult = GetBallotResult();
                return voteResult.First((p) => p.Value > totalVotes / 2.00m).Key;
            }

            public bool HasMajorityCandidate()
            {
                var totalVotes = this.Count();
                var voteResult = GetBallotResult();
                return voteResult.Any((p) => p.Value > totalVotes / 2.00m);
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

            public void DemoteMinorityCandidates()
            {
                var minorityCandidates = this.GetEliminatedCandidates();
                foreach(var minorityCandidate in minorityCandidates)
                {
                    GetIndexesOfCandidate(minorityCandidate).ForEach(p =>
                    {
                        _parentBallotCollection[p].Pop();
                    });
                }
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
    }    
}