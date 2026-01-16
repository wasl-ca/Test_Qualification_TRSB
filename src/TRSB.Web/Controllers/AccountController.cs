using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TRSB.Web.Models;
using TRSB.Web.Services.Interfaces;

public class AccountController : Controller
{
    private readonly IAccountService _auth;

    public AccountController(IAccountService auth)
    {
        _auth = auth;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await _auth.Login(model);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Error!;
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        return RedirectToAction("Index", "Profile");
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _auth.Register(model);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Error!;
            ModelState.AddModelError(string.Empty, result.Error!);
            return View("Register", model);
        }
        TempData["SuccessMessage"] = "Inscription réussie. Veuillez vous connecter.";

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await _auth.Logout();
        return RedirectToAction("Login");
    }
}
