﻿using Bit.Core.Context;
using Bit.Core.Exceptions;
using Bit.Core.Models.Api;
using Bit.Core.NotificationHub;
using Bit.Core.Platform.Push;
using Bit.Core.Settings;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Api.Platform.Push;

/// <summary>
/// Routes for push relay: functionality that facilitates communication
/// between self hosted organizations and Bitwarden cloud.
/// </summary>
[Route("push")]
[Authorize("Push")]
[SelfHosted(NotSelfHostedOnly = true)]
public class PushController : Controller
{
    private readonly IPushRegistrationService _pushRegistrationService;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IWebHostEnvironment _environment;
    private readonly ICurrentContext _currentContext;
    private readonly IGlobalSettings _globalSettings;

    public PushController(
        IPushRegistrationService pushRegistrationService,
        IPushNotificationService pushNotificationService,
        IWebHostEnvironment environment,
        ICurrentContext currentContext,
        IGlobalSettings globalSettings)
    {
        _currentContext = currentContext;
        _environment = environment;
        _pushRegistrationService = pushRegistrationService;
        _pushNotificationService = pushNotificationService;
        _globalSettings = globalSettings;
    }

    [HttpPost("register")]
    public async Task RegisterAsync([FromBody] PushRegistrationRequestModel model)
    {
        CheckUsage();
        await _pushRegistrationService.CreateOrUpdateRegistrationAsync(new PushRegistrationData(model.PushToken),
            Prefix(model.DeviceId), Prefix(model.UserId), Prefix(model.Identifier), model.Type,
            model.OrganizationIds?.Select(Prefix) ?? [], model.InstallationId);
    }

    [HttpPost("delete")]
    public async Task DeleteAsync([FromBody] PushDeviceRequestModel model)
    {
        CheckUsage();
        await _pushRegistrationService.DeleteRegistrationAsync(Prefix(model.Id));
    }

    [HttpPut("add-organization")]
    public async Task AddOrganizationAsync([FromBody] PushUpdateRequestModel model)
    {
        CheckUsage();
        await _pushRegistrationService.AddUserRegistrationOrganizationAsync(
            model.Devices.Select(d => Prefix(d.Id)),
            Prefix(model.OrganizationId));
    }

    [HttpPut("delete-organization")]
    public async Task DeleteOrganizationAsync([FromBody] PushUpdateRequestModel model)
    {
        CheckUsage();
        await _pushRegistrationService.DeleteUserRegistrationOrganizationAsync(
            model.Devices.Select(d => Prefix(d.Id)),
            Prefix(model.OrganizationId));
    }

    [HttpPost("send")]
    public async Task SendAsync([FromBody] PushSendRequestModel model)
    {
        CheckUsage();

        if (!string.IsNullOrWhiteSpace(model.InstallationId))
        {
            if (_currentContext.InstallationId!.Value.ToString() != model.InstallationId!)
            {
                throw new BadRequestException("InstallationId does not match current context.");
            }

            await _pushNotificationService.SendPayloadToInstallationAsync(
                _currentContext.InstallationId.Value.ToString(), model.Type, model.Payload, Prefix(model.Identifier),
                Prefix(model.DeviceId), model.ClientType);
        }
        else if (!string.IsNullOrWhiteSpace(model.UserId))
        {
            await _pushNotificationService.SendPayloadToUserAsync(Prefix(model.UserId),
                model.Type, model.Payload, Prefix(model.Identifier), Prefix(model.DeviceId), model.ClientType);
        }
        else if (!string.IsNullOrWhiteSpace(model.OrganizationId))
        {
            await _pushNotificationService.SendPayloadToOrganizationAsync(Prefix(model.OrganizationId),
                model.Type, model.Payload, Prefix(model.Identifier), Prefix(model.DeviceId), model.ClientType);
        }
    }

    private string Prefix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return $"{_currentContext.InstallationId!.Value}_{value}";
    }

    private void CheckUsage()
    {
        if (CanUse())
        {
            return;
        }

        throw new BadRequestException("Not correctly configured for push relays.");
    }

    private bool CanUse()
    {
        if (_environment.IsDevelopment())
        {
            return true;
        }

        return _currentContext.InstallationId.HasValue && !_globalSettings.SelfHosted;
    }
}
