using PromotionCodes.Db.Entities;
using PromotionCodes.Services;

namespace PromotionCodes.Tests
{
    public class DbFaker : IPromoCodeService
    {

        List<PromoCodeEntry> _entries;
        public DbFaker() {
            _entries = new List<PromoCodeEntry>();
        }

        public Task CreateAsync(PromoCodeEntry newPromoCode)
        {
            _entries.Add(newPromoCode);
            return Task.CompletedTask;
        }

        public Task<List<PromoCodeEntry>> GetAsync() =>
            Task.FromResult(_entries);

        public Task<PromoCodeEntry?> GetAsync(Guid id) =>
            Task.FromResult(_entries.Where(x => x.Id == id).SingleOrDefault());

        public Task RemoveAsync(Guid id) =>
            Task.FromResult(_entries.RemoveAll(x => x.Id == id));

        public Task UpdateAsync(Guid id, PromoCodeEntry promoCodeData)
        {
            _entries.RemoveAll(x => x.Id == id);
            _entries.Add(promoCodeData);
            return Task.CompletedTask;
        }
    }
}
