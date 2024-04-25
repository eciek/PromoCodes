using PromotionCodes.Db.Entities;
using PromotionCodes.Models;
using PromotionCodes.Services;

namespace PromotionCodes.Handlers;

public class PromoCodeHandler : IPromoCodeHandler
{
    readonly ILogger<PromoCodeHandler> _logger;
    readonly IPromoCodeService _promoCodeService;

    public PromoCodeHandler(ILogger<PromoCodeHandler> logger,
        IPromoCodeService promoCodeService)
    {
        _logger = logger;
        _promoCodeService = promoCodeService;
    }

    public async Task<Guid> CreateNewEntryAsync(PromoCode promoCode)
    {

        if (promoCode is null || string.IsNullOrEmpty(promoCode.Name))
            throw new ArgumentNullException(nameof(promoCode));

        PromoCodeEntry promoCodeEntry = new PromoCodeEntry()
        {
            Id = Guid.NewGuid(),
            IsActive = promoCode.IsActive ?? true,
            Name = promoCode.Name,
            MaxUses = promoCode.MaxUses ?? 1,
            PromoCodeHistoryEntries = new List<PromoCodeHistoryEntry>(),
            PromoCodeRedeems = new List<PromoCodeRedeem>()

        };

        promoCodeEntry.RegisterHistoryEntry();
        await _promoCodeService.CreateAsync(promoCodeEntry);

        _logger.LogInformation($"Created new promo code. Code Name: [{promoCode.Name}], Uses:[{promoCode.MaxUses}], Active:[{promoCode.IsActive}]");
        return promoCodeEntry.Id;
    }

    public async Task UpdateEntryAsync(Guid promoCodeId, PromoCode promoCode)
    {
        if (promoCodeId == Guid.Empty)
            throw new ArgumentNullException(nameof(promoCodeId));

        if (promoCode is null)
            throw new ArgumentNullException(nameof(promoCode));

        var promoCodeEntry = await _promoCodeService.GetAsync(promoCodeId) 
            ?? throw new KeyNotFoundException(nameof(promoCodeId));

        promoCodeEntry.IsActive = promoCode.IsActive ?? promoCodeEntry.IsActive;
        promoCodeEntry.MaxUses = promoCode.MaxUses ?? promoCodeEntry.MaxUses;
        promoCodeEntry.Name = !string.IsNullOrEmpty(promoCode.Name) ? promoCode.Name : promoCodeEntry.Name;

        promoCodeEntry!.RegisterHistoryEntry();
        await _promoCodeService.UpdateAsync(promoCodeEntry.Id, promoCodeEntry);

        _logger.LogInformation($"Updated Code with id [{promoCodeEntry.Id}] to following values: Code Name: [{promoCode.Name}], Uses: [{promoCode.MaxUses}], Active: [{promoCode.IsActive}]");
    }

    public async Task DeleteEntry(Guid promoCodeId)
    {
        if (promoCodeId == Guid.Empty)
            throw new ArgumentNullException(nameof(promoCodeId));

        await _promoCodeService.RemoveAsync(promoCodeId);
        _logger.LogInformation($"Deleted {promoCodeId}");
    }

    public async Task<string?> ViewCode(Guid promoCodeId)
    {
        if (promoCodeId == Guid.Empty)
            throw new ArgumentNullException(nameof(promoCodeId));

        var promoCodeEntry = await _promoCodeService.GetAsync(promoCodeId)
          ?? throw new KeyNotFoundException(nameof(promoCodeId));

        if (promoCodeEntry.IsActive == false)
        {
            var errmsg = $"Code {promoCodeId} is not active!";
            
            _logger.LogWarning(errmsg);
            throw new InvalidOperationException(errmsg);
        }

        if (promoCodeEntry.MaxUses <= promoCodeEntry.PromoCodeRedeems?.Count)
        {
            var errmsg = $"Code {promoCodeId} has been viewed too many times!";

            _logger.LogWarning(errmsg);
            throw new InvalidOperationException(errmsg);
        }

        PromoCodeRedeem promoCodeRedeem = new PromoCodeRedeem()
        {
            Id = new Guid(),
            RedeemDate = DateTime.Now,
        };

        promoCodeEntry.PromoCodeRedeems?.Add(promoCodeRedeem);
        await _promoCodeService.UpdateAsync(promoCodeEntry.Id, promoCodeEntry);

        _logger.LogInformation($"Viewing of code {promoCodeId} has been registered!");

        return promoCodeEntry.Name;
    }
}

public static class LocalExtensions
{
    public static void RegisterHistoryEntry(this PromoCodeEntry promoCode)
    {
        var historyEntry = new PromoCodeHistoryEntry()
        {
            IsActive = promoCode.IsActive,
            Name = promoCode.Name,
            MaxUses = promoCode.MaxUses,

            ChangeDate = DateTime.Now,
            Version = promoCode.PromoCodeHistoryEntries?.Count
        };

        promoCode.PromoCodeHistoryEntries?.Add(historyEntry);
    }
}
