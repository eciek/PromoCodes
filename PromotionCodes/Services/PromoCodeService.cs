using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PromotionCodes.Db;
using PromotionCodes.Db.Entities;

namespace PromotionCodes.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IMongoCollection<PromoCodeEntry> _promoCodeCollection;

    public PromoCodeService(IOptions<PromoCodeDbSettings> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);

        _promoCodeCollection = mongoDatabase.GetCollection<PromoCodeEntry>(options.Value.PromotionCollectionName);
    }

    public async Task<List<PromoCodeEntry>> GetAsync() =>
        await _promoCodeCollection.Find(_ => true).ToListAsync();

    public async Task<PromoCodeEntry?> GetAsync(Guid id) =>
        await _promoCodeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(PromoCodeEntry newPromoCode) =>
        await _promoCodeCollection.InsertOneAsync(newPromoCode);

    public async Task UpdateAsync(Guid id, PromoCodeEntry updatedPromoCode) =>
        await _promoCodeCollection.ReplaceOneAsync(x => x.Id == id, updatedPromoCode);

    public async Task RemoveAsync(Guid id) =>
        await _promoCodeCollection.DeleteOneAsync(x => x.Id == id);

}
