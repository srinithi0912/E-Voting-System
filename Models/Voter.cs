using System.ComponentModel.DataAnnotations;

namespace EVotingSystem.Models
{
    public class Voter
    {
        [Key]
        public int VoterId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Password { get; set; } = string.Empty;

        public bool HasVoted { get; set; } = false;
    }
}