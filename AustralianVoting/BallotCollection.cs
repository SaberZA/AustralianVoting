using System.Collections.Generic;
using System.Linq;

namespace AustralianVoting
{
    internal partial class Program
    {
        public class BallotCollection : List<Stack<int>>
        {
            public BallotCollection(IEnumerable<Stack<int>> voterChoices) : base(voterChoices)
            {

            }

            public BallotRound GetCurrentBallotRound()
            {
                return new BallotRound(this.Select(p => p.FirstOrDefault())
                    .ToList(), this);
            }

            public List<int> CountAllVotes()
            {
                return TallyVotes(GetCurrentBallotRound());
            }

            private List<int> TallyVotes(BallotRound ballotRound)
            {
                if (ballotRound.Count == 0) return new List<int>();
                if (ballotRound.Count == 1) return new List<int> { ballotRound.First() };
                if (ballotRound.HasMajorityCandidate()) return new List<int> { ballotRound.GetMajorityCandidate() };
                if (ballotRound.AllCandidatesTied()) return ballotRound.GetRemainingCandidates();

                ballotRound.DemoteMinorityCandidates();

                return TallyVotes(GetCurrentBallotRound());
            }
        }
    }    
}