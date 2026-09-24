using Microsoft.EntityFrameworkCore;
using TranslationBureau.Application.Common;
using TranslationBureau.Application.DTOs;
using TranslationBureau.Application.Interfaces;
using TranslationBureau.Application.Mapping;
using TranslationBureau.Domain.Entities;
using TranslationBureau.Domain.Exceptions;
using TranslationBureau.Domain.Interfaces;

namespace TranslationBureau.Application.Services;

public class TranslatorService : ITranslatorService
{
    private readonly ITranslatorRepository _repository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TranslatorService(ITranslatorRepository repository,
        ILanguageRepository languageRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TranslatorDto>> GetPagedAsync(
        TranslatorFilterDto filter, CancellationToken cancellationToken = default)
    {
        IQueryable<Translator> query = _repository.Query()
            .Include(t => t.Languages)
                .ThenInclude(tl => tl.Language);

        // Поиск по части ФИО
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = filter.Search.Trim();
            query = query.Where(t => EF.Functions.Like(t.FullName, $"%{pattern}%"));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(t => t.Category == filter.Category);

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive.Value);

        query = filter.SortOrder switch
        {
            "name_desc" => query.OrderByDescending(t => t.FullName),
            "rate" => query.OrderBy(t => t.RatePerUnit),
            "rate_desc" => query.OrderByDescending(t => t.RatePerUnit),
            _ => query.OrderBy(t => t.FullName)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TranslatorDto>
        {
            Items = items.Select(t => t.ToDto()).ToList(),
            PageIndex = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
        => await _repository.Query()
            .Where(t => t.Category != null)
            .Select(t => t.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

    public async Task<TranslatorDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var translator = await _repository.GetByIdAsync(id, cancellationToken);
        return translator?.ToDto();
    }

    public async Task<TranslatorInputDto?> GetForEditAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var translator = await _repository.GetByIdAsync(id, cancellationToken);
        return translator?.ToInputDto();
    }

    public async Task<int> CreateAsync(TranslatorInputDto dto,
        CancellationToken cancellationToken = default)
    {
        var translator = Translator.Create(dto.FullName, dto.Phone,
            dto.Email, dto.Category, dto.RatePerUnit);

        var languages = await LoadLanguagesAsync(dto.LanguageIds, cancellationToken);
        translator.SetLanguages(languages);

        await _repository.AddAsync(translator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return translator.Id;
    }

    public async Task UpdateAsync(TranslatorInputDto dto,
        CancellationToken cancellationToken = default)
    {
        var translator = await _repository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new DomainException("Переводчик не найден.");

        translator.Update(dto.FullName, dto.Phone, dto.Email,
            dto.Category, dto.RatePerUnit);

        var languages = await LoadLanguagesAsync(dto.LanguageIds, cancellationToken);
        translator.SetLanguages(languages);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var translator = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Переводчик не найден.");

        translator.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var translator = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Переводчик не найден.");

        translator.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LanguageDto>> GetAllLanguagesAsync(
        CancellationToken cancellationToken = default)
        => await _languageRepository.Query()
            .OrderBy(l => l.Name)
            .Select(l => l.ToDto())
            .ToListAsync(cancellationToken);

    private async Task<List<Language>> LoadLanguagesAsync(
        List<int> ids, CancellationToken cancellationToken)
    {
        if (ids is null || ids.Count == 0)
            throw new DomainException("Переводчик должен владеть хотя бы одним рабочим языком.");

        var languages = await _languageRepository.Query()
            .Where(l => ids.Contains(l.Id))
            .ToListAsync(cancellationToken);

        if (languages.Count != ids.Distinct().Count())
            throw new DomainException("Один или несколько языков не найдены.");

        return languages;
    }
}