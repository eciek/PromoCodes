using Microsoft.AspNetCore.Mvc;
using PromotionCodes.Db.Entities;
using PromotionCodes.Handlers;
using PromotionCodes.Models;
using PromotionCodes.Services;


namespace PromotionCodes.Controllers;

[ApiController]
[Route("[controller]")]
public class PromoCodeController : ControllerBase
{
    IPromoCodeHandler _promoCodeHandler;
    IPromoCodeService _promoCodeService;

    public PromoCodeController(IPromoCodeHandler handler, IPromoCodeService service)
    {
        _promoCodeHandler = handler;
        _promoCodeService = service;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<PromoCodeEntry>> Get([FromRoute] Guid id)
    {
        var promocode = await _promoCodeService.GetAsync(id);
        if (promocode is not null)
            return Ok(promocode);
        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<List<PromoCodeEntry>>> Get()
        => await _promoCodeService.GetAsync();

    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromBody] PromoCode promoCode)
    {
        try
        {
            var promoCodeId = await _promoCodeHandler.CreateNewEntryAsync(promoCode);
            return Ok(promoCodeId);
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<ActionResult> Patch([FromRoute] Guid id, [FromBody] PromoCode promoCode)
    {
        try
        {
            await _promoCodeHandler.UpdateEntryAsync(id, promoCode);

            return Ok();
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch]
    [Route("{id}/deactivate")]
    public async Task<ActionResult> DeactivateCode([FromRoute] Guid id)
        => await Patch(id, new PromoCode() { IsActive = false });

    [HttpPatch]
    [Route("{id}/rename")]
    public async Task<ActionResult> RenameCode([FromRoute] Guid id, [FromBody]string name)
        => await Patch(id, new PromoCode() { Name = name });

    [HttpPost]
    [Route("{id}/view")]
    public async Task<ActionResult<string>> ViewCode([FromRoute] Guid id)
    {
        try
        {
            var codeName = await _promoCodeHandler.ViewCode(id);
            return Ok(codeName);
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest("Failed to view code.\n" + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task <ActionResult> Delete([FromRoute] Guid id)
    {
        await _promoCodeHandler.DeleteEntry(id);
        return Ok();
    }
}
