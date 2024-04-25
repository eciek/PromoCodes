using Amazon.Runtime.Internal.Util;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Logging;
using Moq;
using PromotionCodes.Db.Entities;
using PromotionCodes.Handlers;
using PromotionCodes.Models;
using PromotionCodes.Services;
using SharpCompress.Common;

namespace PromotionCodes.Tests;

public class PromoCodeHandlerTests
{
    private readonly DbFaker _dbFaker = new DbFaker(); 
    private readonly Mock<ILogger<PromoCodeHandler>> _loggerMock = new Mock<ILogger<PromoCodeHandler>>();

    private readonly PromoCodeEntry ValidEntry = new() {Id = Guid.NewGuid(), Name = "Existingcode",MaxUses =  5, IsActive = true};
    private readonly PromoCodeEntry DisabledEntry = new() { Id = Guid.NewGuid(), Name = "Disabledcode", MaxUses = 5, IsActive = false };
    private readonly PromoCodeEntry OutOfUseEntry = new() {Id = Guid.NewGuid(),Name = "Overusedcode", MaxUses = 2, IsActive = true,
        PromoCodeRedeems = new List<PromoCodeRedeem>() {
            new PromoCodeRedeem(){Id = Guid.NewGuid(), RedeemDate = DateTime.Now},
            new PromoCodeRedeem(){Id = Guid.NewGuid(), RedeemDate = DateTime.Now},
        }
    };


    [Fact]
    public async void ShouldCreateNewEntry()
    {
        //arrange
        Setup();
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker);

        var newCode = new PromoCode()
        {
            Name = "Test",
            MaxUses = 1,
            IsActive = true,
        };

        //act
        var newCodeId = await promoCodeHandler.CreateNewEntryAsync(newCode);

        //assert
        var newEntry = await _dbFaker.GetAsync(newCodeId) ;
        Assert.Equal(newCodeId, newEntry?.Id);
    }

    [Fact]
    public async void ShouldThrowWhenNameIsMissing()
    {
        //arrange
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker); 
        Setup();

        var newCode = new PromoCode()
        {
            Name = string.Empty,
            MaxUses = 1,
            IsActive = true,
        };

        //act
        //assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() =>
        {
            return promoCodeHandler.CreateNewEntryAsync(newCode);
        });
    }

    [Fact]
    public async void ShouldDeleteEntry()
    {
        //arrange
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker); 
        Setup();
        
        // act
        await promoCodeHandler.DeleteEntry(ValidEntry.Id);

        //assert
        Assert.Null(await _dbFaker.GetAsync(ValidEntry.Id));
    }

    [Fact]
    public async void ShouldUpdateEntry()
    {
        //arrange
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker);
        Setup();

        var updateData = new PromoCode()
        {
            Name = "newName",
            IsActive = false,
        };

        //act
        await promoCodeHandler.UpdateEntryAsync(ValidEntry.Id, updateData);


        //assert
        var updatedEntry = await _dbFaker.GetAsync(ValidEntry.Id) as PromoCode;

        Assert.Equal(updateData.Name, updatedEntry?.Name);
        Assert.Equal(updateData.IsActive, updatedEntry?.IsActive);
    }

    [Fact]
    public async void ShouldViewCode()
    {
        //arrange
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker); 
        Setup();

        //act
        var code = await promoCodeHandler.ViewCode(ValidEntry.Id);

        //assert
        Assert.Equal(code, ValidEntry.Name);
    }

    [Fact]
    public async void ShouldThrowWhenViewingInvalidCode()
    {
        //arrange
        var promoCodeHandler = new PromoCodeHandler(_loggerMock.Object, _dbFaker); 
        Setup();

        //act
        //assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>
        {
            return promoCodeHandler.ViewCode(DisabledEntry.Id);
        });

        await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>
        {
            return promoCodeHandler.ViewCode(OutOfUseEntry.Id);
        });

        await Assert.ThrowsAnyAsync<ArgumentNullException>(() =>
        {
            return promoCodeHandler.ViewCode(Guid.Empty);
        });

        await Assert.ThrowsAnyAsync<KeyNotFoundException>(() =>
        {
            return promoCodeHandler.ViewCode(Guid.NewGuid());
        });

    }

    private void Setup()
    {
        _dbFaker.CreateAsync(ValidEntry);
        _dbFaker.CreateAsync(DisabledEntry);
        _dbFaker.CreateAsync(OutOfUseEntry);
    }

}