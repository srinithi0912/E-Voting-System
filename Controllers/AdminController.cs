using Microsoft.AspNetCore.Mvc;
using EVotingSystem.Data;
using EVotingSystem.Models;

namespace EVotingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var results = _context.Candidate
                .Select(c => new CandidateResultViewModel
                {
                    CandidateId = c.CandidateId,
                    CandidateName = c.Name,
                    PartyName = c.Party,
                    VoteCount = _context.Vote.Count(v => v.CandidateId == c.CandidateId)
                }).ToList();

            var winner = results.OrderByDescending(r => r.VoteCount).FirstOrDefault();
            var runnerUp = results.OrderByDescending(r => r.VoteCount).Skip(1).FirstOrDefault();

            ViewBag.WinnerParty = winner != null && winner.VoteCount > 0 ? winner.PartyName : "N/A";
            ViewBag.OpponentParty = runnerUp != null && runnerUp.VoteCount > 0 ? runnerUp.PartyName : "N/A";

            return View(results);
        }

        [HttpPost]
        public IActionResult AddCandidate(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                _context.Candidate.Add(candidate);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult RemoveCandidate(int candidateId)
        {
            var candidate = _context.Candidate.Find(candidateId);
            if (candidate != null)
            {
                _context.Candidate.Remove(candidate);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }

    public class CandidateResultViewModel
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
    }
}