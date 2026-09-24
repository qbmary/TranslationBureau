using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TranslationBureau.Application.DTOs;
using TranslationBureau.Application.Interfaces;
using TranslationBureau.Domain.Exceptions;
using TranslationBureau.Web.ViewModels;

namespace TranslationBureau.Web.Controllers;

public class TranslatorsController : Controller
{
    private readonly ITranslatorService _service;

    public TranslatorsController(ITranslatorService service) => _service = service;

    public async Task<IActionResult> Index([FromQuery] TranslatorFilterDto filter,
        CancellationToken cancellationToken)
    {
        var model = new TranslatorIndexViewModel
        {
            Translators = await _service.GetPagedAsync(filter, cancellationToken),
            Filter = filter,
            Categories = await _service.GetCategoriesAsync(cancellationToken)
        };
        return View(model);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadLanguagesAsync(cancellationToken);
        return View(new TranslatorInputDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TranslatorInputDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadLanguagesAsync(cancellationToken);
            return View(model);
        }

        try
        {
            await _service.CreateAsync(model, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadLanguagesAsync(cancellationToken);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetForEditAsync(id, cancellationToken);
        if (model is null)
            return NotFound();

        await LoadLanguagesAsync(cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TranslatorInputDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadLanguagesAsync(cancellationToken);
            return View(model);
        }

        try
        {
            await _service.UpdateAsync(model, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadLanguagesAsync(cancellationToken);
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id,
        CancellationToken cancellationToken)
    {
        await _service.DeactivateAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Возвращает переводчика из архивного состояния.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
    {
        await _service.RestoreAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Кладёт в ViewBag список языков для multi-select.</summary>
    private async Task LoadLanguagesAsync(CancellationToken cancellationToken)
    {
        var languages = await _service.GetAllLanguagesAsync(cancellationToken);
        ViewBag.Languages = languages
            .Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.Name} ({l.Code})"
            })
            .ToList();
    }
}