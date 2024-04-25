using PromotionCodes.Models;

namespace PromotionCodes.Db.Entities
{
    public class PromoCodeHistoryEntry : PromoCode
    {
        public int? Version { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
