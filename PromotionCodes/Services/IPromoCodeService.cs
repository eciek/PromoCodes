using PromotionCodes.Db.Entities;
using PromotionCodes.Models;

namespace PromotionCodes.Services
{
    public interface IPromoCodeService
    {
        public Task<List<PromoCodeEntry>> GetAsync();

        public Task<PromoCodeEntry?> GetAsync(Guid id);

        public Task CreateAsync(PromoCodeEntry newPromoCode);

        public Task UpdateAsync(Guid id, PromoCodeEntry promoCodeData);

        public Task RemoveAsync(Guid id);
    }
}