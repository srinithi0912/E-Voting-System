using System.ComponentModel.DataAnnotations;

namespace EVotingSystem.Models
{
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Party { get; set; } = string.Empty;
    }
}