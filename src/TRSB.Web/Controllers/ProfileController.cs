using Microsoft.AspNetCore.Mvc;
using TRSB.Web.Models;
using TRSB.Web.Services.Interfaces;

public class ProfileController: Controller
{
    private readonly IAccountService _profile;
    public ProfileController(IAccountService profile)
    {
        _profile = profile;
    }
    public async Task<IActionResult> Index()
    {
        var profile = await _profile.GetProfileAsync();
        if(!profile.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, profile.Error!);
        }
        return View(profile.Value);
    }

    public async Task<IActionResult> Edit()
    {
        var profile = await _profile.GetProfileAsync();
        var UpdateModel = new UpdateProfileViewModel
        {
            Username = profile.Value!.Username,
            Name = profile.Value.Name,
            Email = profile.Value.Email,
            Password = string.Empty,
            ConfirmPassword = string.Empty
        };
        return View("Edit", UpdateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateProfileViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View("Edit", model);
        }
        var result = await _profile.UpdateProfileAsync(model);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Error!;
            ModelState.AddModelError(string.Empty, result.Error!);
            return View("Edit", model);
        }

        TempData["SuccessMessage"] = "Profil mis à jour avec succès";

        return RedirectToAction("Index");
    }
}