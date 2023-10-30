﻿using Bit.Api.Vault;
using Bit.Api.Vault.Models.Request;
using Bit.Core.Exceptions;
using Bit.Core.Vault.Models.Data;
using Bit.Core.Vault.Repositories;
using Bit.Test.Common.AutoFixture;
using Bit.Test.Common.AutoFixture.Attributes;
using NSubstitute;
using Xunit;

namespace Bit.Api.Test.Vault.Validators;


[SutProviderCustomize]
public class CipherRotationValidatorTests
{
    [Theory, BitAutoData]
    public async Task ValidateAsync_MissingCipher_Throws(SutProvider<CipherRotationValidator> sutProvider, Guid userId, IEnumerable<CipherWithIdRequestModel> ciphers)
    {
        var userCiphers = ciphers.Select(c => new CipherDetails { Id = c.Id.GetValueOrDefault(), Type = c.Type }).ToList();
        userCiphers.Add(new CipherDetails { Id = Guid.NewGuid(), Type = Core.Vault.Enums.CipherType.Login });
        sutProvider.GetDependency<ICipherRepository>().GetManyByUserIdAsync(userId).Returns(userCiphers);


        await Assert.ThrowsAsync<BadRequestException>(async () => await sutProvider.Sut.ValidateAsync(userId, ciphers));
    }

    [Theory, BitAutoData]
    public async Task ValidateAsync_CipherDoesNotBelongToUser_NotIncluded(SutProvider<CipherRotationValidator> sutProvider, Guid userId, IEnumerable<CipherWithIdRequestModel> ciphers)
    {
        var userCiphers = ciphers.Select(c => new CipherDetails { Id = c.Id.GetValueOrDefault(), Type = c.Type }).ToList();
        userCiphers.RemoveAt(0);
        sutProvider.GetDependency<ICipherRepository>().GetManyByUserIdAsync(userId).Returns(userCiphers);

        var result = await sutProvider.Sut.ValidateAsync(userId, ciphers);

        Assert.DoesNotContain(result, c => c.Id == ciphers.First().Id);
    }


}
