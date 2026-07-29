using FreelanceHub.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers;

public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Not logged in
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Browse", "Job");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Browse", "Job");
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        if (await _userManager.IsInRoleAsync(user, "Client"))
        {
            return RedirectToAction("MyJobs", "Job");
            // or ClientDashboard if you have one
        }

        if (await _userManager.IsInRoleAsync(user, "Freelancer"))
        {
            return RedirectToAction("Browse", "Job");
            // or FreelancerDashboard
        }

        return RedirectToAction("Browse", "Job");
    }
}