using PromotionCodes.Db.Entities;
using PromotionCodes.Models;

namespace PromotionCodes.Handlers;

public interface IPromoCodeHandler
{
    Task<Guid> CreateNewEntryAsync(PromoCode promoCode);

    Task UpdateEntryAsync(Guid id, PromoCode promoCode);

    Task<string?> ViewCode(Guid promoCodeId);

    Task DeleteEntry(Guid promoCodeId);
}