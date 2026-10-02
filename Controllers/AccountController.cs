using Microsoft.AspNetCore.Mvc;
using EVotingSystem.Data;
using EVotingSystem.Models;

namespace EVotingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User Type Selection (Main Entry Page)
        public IActionResult Login()
        {
            return View();
        }

        // Direct Action for Admin Button (No Formal Login Required)
        public IActionResult AdminLogin()
        {
            HttpContext.Session.SetString("UserRole", "Admin");
            return RedirectToAction("Dashboard", "Admin");
        }

        // GET: Voter Login Page
        public IActionResult VoterLogin()
        {
            return View();
        }

        // POST: Voter Login Process
        [HttpPost]
        public IActionResult VoterLogin(int voterId, string password)
        {
            var voter = _context.Voter.FirstOrDefault(v => v.VoterId == voterId && v.Password == password);
            if (voter != null)
            {
                HttpContext.Session.SetInt32("VoterId", voter.VoterId);
                HttpContext.Session.SetString("VoterName", voter.Name);
                HttpContext.Session.SetString("UserRole", "Voter");
                return RedirectToAction("Dashboard", "Voter");
            }

            ViewBag.Error = "Invalid Voter ID or Password";
            return View();
        }

        // GET: Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register Process
        [HttpPost]
        public IActionResult Register(Voter voter)
        {
            if (ModelState.IsValid)
            {
                _context.Voter.Add(voter);
                _context.SaveChanges();
                TempData["Success"] = $"Registered successfully! Your Voter ID is {voter.VoterId}";
                return RedirectToAction("VoterLogin");
            }
            return View(voter);
        }

        // Logout Action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}