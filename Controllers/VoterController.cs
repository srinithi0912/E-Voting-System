using Microsoft.AspNetCore.Mvc;
using EVotingSystem.Data;
using EVotingSystem.Models;

namespace EVotingSystem.Controllers
{
    public class VoterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Voter/Dashboard
        public IActionResult Dashboard(string searchString)
        {
            int? voterId = HttpContext.Session.GetInt32("VoterId");
            if (voterId == null)
            {
                return RedirectToAction("VoterLogin", "Account");
            }

            var voter = _context.Voter.Find(voterId);
            ViewBag.HasVoted = voter?.HasVoted ?? false;
            ViewBag.VoterName = voter?.Name ?? "Voter";
            ViewBag.VoterLocation = HttpContext.Session.GetString("VoterLocation") ?? "Main Station";

            // 7:00 AM to 5:00 PM Time Window
            DateTime now = DateTime.Now;
            TimeSpan startTime = new TimeSpan(7, 0, 0);
            TimeSpan endTime = new TimeSpan(17, 0, 0);
            bool isVotingTime = now.TimeOfDay >= startTime && now.TimeOfDay <= endTime;
            ViewBag.IsVotingEligible = isVotingTime;

            // Fetch candidates safely
            List<Candidate> candidates = new List<Candidate>();
            if (_context.Candidate != null)
            {
                var query = _context.Candidate.AsQueryable();
                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(c => c.Name.Contains(searchString) || c.Party.Contains(searchString));
                }
                candidates = query.ToList();
            }

            return View(candidates);
        }

        [HttpPost]
        public IActionResult CastVote(int candidateId)
        {
            int? voterId = HttpContext.Session.GetInt32("VoterId");
            if (voterId == null) return RedirectToAction("VoterLogin", "Account");

            DateTime now = DateTime.Now;
            TimeSpan startTime = new TimeSpan(7, 0, 0);
            TimeSpan endTime = new TimeSpan(17, 0, 0);

            if (now.TimeOfDay < startTime || now.TimeOfDay > endTime)
            {
                TempData["VoteStatus"] = "Voting is closed! Hours are 7:00 AM to 5:00 PM.";
                return RedirectToAction("Dashboard");
            }

            var voter = _context.Voter.Find(voterId);
            if (voter != null && !voter.HasVoted)
            {
                string location = HttpContext.Session.GetString("VoterLocation") ?? "Main Station";

                var vote = new Vote
                {
                    VoterId = voter.VoterId,
                    CandidateId = candidateId,
                    Location = location,
                    VotedAt = DateTime.Now
                };

                voter.HasVoted = true;
                _context.Vote.Add(vote);
                _context.SaveChanges();

                TempData["VoteStatus"] = "Vote Successfully Casted!";
            }
            else
            {
                TempData["VoteStatus"] = "You have already casted your vote.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult UpdateProfile(string name, string location, string password)
        {
            int? voterId = HttpContext.Session.GetInt32("VoterId");
            if (voterId == null) return RedirectToAction("VoterLogin", "Account");

            var voter = _context.Voter.Find(voterId);
            if (voter != null)
            {
                voter.Name = name;
                if (!string.IsNullOrEmpty(password)) voter.Password = password;

                _context.SaveChanges();

                if (!string.IsNullOrEmpty(location))
                {
                    HttpContext.Session.SetString("VoterLocation", location);
                }

                HttpContext.Session.SetString("VoterName", voter.Name);
            }

            return RedirectToAction("Dashboard");
        }
    }
}