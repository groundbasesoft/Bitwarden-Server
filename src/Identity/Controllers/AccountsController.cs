﻿using Bit.Core;
using Bit.Core.Auth.Enums;
using Bit.Core.Auth.Models.Api.Request.Accounts;
using Bit.Core.Auth.Models.Api.Response.Accounts;
using Bit.Core.Auth.Models.Business.Tokenables;
using Bit.Core.Auth.Services;
using Bit.Core.Auth.Utilities;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using Bit.Core.Models.Data;
using Bit.Core.Repositories;
using Bit.Core.Services;
using Bit.Core.Tokens;
using Bit.Core.Utilities;
using Bit.SharedWeb.Utilities;
using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Identity.Controllers;

[Route("accounts")]
[ExceptionHandlerFilter]
public class AccountsController : Controller
{
    private readonly ILogger<AccountsController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly ICaptchaValidationService _captchaValidationService;
    private readonly IDataProtectorTokenFactory<WebAuthnLoginAssertionOptionsTokenable> _assertionOptionsDataProtector;


    public AccountsController(
        ILogger<AccountsController> logger,
        IUserRepository userRepository,
        IUserService userService,
        ICaptchaValidationService captchaValidationService,
        IDataProtectorTokenFactory<WebAuthnLoginAssertionOptionsTokenable> assertionOptionsDataProtector)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userService = userService;
        _captchaValidationService = captchaValidationService;
        _assertionOptionsDataProtector = assertionOptionsDataProtector;
    }

    // Moved from API, If you modify this endpoint, please update API as well. Self hosted installs still use the API endpoints.
    [HttpPost("register")]
    [CaptchaProtected]
    public async Task<RegisterResponseModel> PostRegister([FromBody] RegisterRequestModel model)
    {
        var user = model.ToUser();
        var result = await _userService.RegisterUserAsync(user, model.MasterPasswordHash,
            model.Token, model.OrganizationUserId);
        if (result.Succeeded)
        {
            var captchaBypassToken = _captchaValidationService.GenerateCaptchaBypassToken(user);
            return new RegisterResponseModel(captchaBypassToken);
        }

        foreach (var error in result.Errors.Where(e => e.Code != "DuplicateUserName"))
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        await Task.Delay(2000);
        throw new BadRequestException(ModelState);
    }

    // Moved from API, If you modify this endpoint, please update API as well. Self hosted installs still use the API endpoints.
    [HttpPost("prelogin")]
    public async Task<PreloginResponseModel> PostPrelogin([FromBody] PreloginRequestModel model)
    {
        var kdfInformation = await _userRepository.GetKdfInformationByEmailAsync(model.Email);
        if (kdfInformation == null)
        {
            kdfInformation = new UserKdfInformation
            {
                Kdf = KdfType.PBKDF2_SHA256,
                KdfIterations = 100000,
            };
        }
        return new PreloginResponseModel(kdfInformation);
    }

    [HttpPost("webauthn/assertion-options")]
    [ApiExplorerSettings(IgnoreApi = true)] // Disable Swagger due to CredentialCreateOptions not converting properly
    [RequireFeature(FeatureFlagKeys.PasswordlessLogin)]
    // TODO: Add tests
    // TODO: Create proper models for this call
    public async Task<WebAuthnLoginAssertionOptionsResponseModel> PostWebAuthnLoginAssertionOptions(WebAuthnLoginAssertionOptionsRequestModel model)
    {
        //var user = await _userRepository.GetByEmailAsync(model.Email);
        //if (user == null)
        //{
        //    // TODO: return something? possible enumeration attacks with this response
        //    return new AssertionOptions();
        //}

        var options = await _userService.StartWebAuthnLoginAssertionAsync();

        var tokenable = new WebAuthnLoginAssertionOptionsTokenable(WebAuthnLoginAssertionOptionsScope.Authentication, options);
        var token = _assertionOptionsDataProtector.Protect(tokenable);

        return new WebAuthnLoginAssertionOptionsResponseModel
        {
            Options = options,
            Token = token
        };
    }

    // TODO: Remove, this will move to the grant validator
    //[HttpPost("webauthn-assertion")]
    //[RequireFeature(FeatureFlagKeys.PasswordlessLogin)]
    //// TODO: Create proper models for this call
    //public async Task<string> PostWebAuthnAssertion([FromBody] PreloginRequestModel model)
    //{
    //    var user = await _userRepository.GetByEmailAsync(model.Email);
    //    if (user == null)
    //    {
    //        // TODO: proper response here?
    //        throw new BadRequestException();
    //    }

    //    var token = await _userService.CompleteWebAuthLoginAssertionAsync(null, user);
    //    return token;
    //}
}
