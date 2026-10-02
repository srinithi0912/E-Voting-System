using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVotingSystem.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        public int VoterId { get; set; }

        public int CandidateId { get; set; }

        [Required, StringLength(150)]
        public string Location { get; set; } = string.Empty;

        public DateTime VotedAt { get; set; } = DateTime.Now;

        [ForeignKey("VoterId")]
        public virtual Voter? Voter { get; set; }

        [ForeignKey("CandidateId")]
        public virtual Candidate? Candidate { get; set; }
    }
}