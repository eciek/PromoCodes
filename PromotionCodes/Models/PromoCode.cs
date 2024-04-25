using System.ComponentModel.DataAnnotations;

namespace PromotionCodes.Models;

public class PromoCode
{
    public string? Name { get; set; }
    public int? MaxUses { get; set; }
    public bool? IsActive { get; set; }
}
