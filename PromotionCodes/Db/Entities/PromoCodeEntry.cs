using PromotionCodes.Models;

namespace PromotionCodes.Db.Entities;

public class PromoCodeEntry : PromoCode
{
    public Guid Id { get; init; }

    public List<PromoCodeHistoryEntry>? PromoCodeHistoryEntries { get; set; }
    public List<PromoCodeRedeem>? PromoCodeRedeems { get; set; }
}
