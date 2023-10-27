﻿using Bit.Core.AdminConsole.Entities;
using Bit.Core.AdminConsole.Entities.Provider;
using Bit.Core.Entities;
using Bit.Core.Enums;
using Bit.Core.SecretsManager.Entities;
using Bit.Core.Vault.Entities;

namespace Bit.Core.Services;

public interface IEventService
{
    Task LogUserEventAsync(Guid userId, EventType type, DateTime? date = null);
    Task LogCipherEventAsync(Cipher cipher, EventType type, DateTime? date = null);
    Task LogCipherEventsAsync(IEnumerable<Tuple<Cipher, EventType, DateTime?>> events);
    Task LogCollectionEventAsync(Collection collection, EventType type, DateTime? date = null);
    Task LogCollectionEventsAsync(IEnumerable<(Collection collection, EventType type, DateTime? date)> events);
    Task LogGroupEventAsync(Group group, EventType type, DateTime? date = null);
    Task LogGroupEventAsync(Group group, EventType type, EventSystemUser systemUser, DateTime? date = null);
    Task LogGroupEventsAsync(IEnumerable<(Group group, EventType type, EventSystemUser? systemUser, DateTime? date)> events);
    Task LogPolicyEventAsync(Policy policy, EventType type, DateTime? date = null);
    Task LogOrganizationUserEventAsync(OrganizationUser organizationUser, EventType type, DateTime? date = null);
    Task LogOrganizationUserEventAsync(OrganizationUser organizationUser, EventType type, EventSystemUser systemUser, DateTime? date = null);
    Task LogOrganizationUserEventsAsync(IEnumerable<(OrganizationUser, EventType, DateTime?)> events);
    Task LogOrganizationUserEventsAsync(IEnumerable<(OrganizationUser, EventType, EventSystemUser, DateTime?)> events);
    Task LogOrganizationEventAsync(Organization organization, EventType type, DateTime? date = null);
    Task LogProviderUserEventAsync(ProviderUser providerUser, EventType type, DateTime? date = null);
    Task LogProviderUsersEventAsync(IEnumerable<(ProviderUser, EventType, DateTime?)> events);
    Task LogProviderOrganizationEventAsync(ProviderOrganization providerOrganization, EventType type, DateTime? date = null);
    Task LogProviderOrganizationEventsAsync(IEnumerable<(ProviderOrganization, EventType, DateTime?)> events);
    Task LogOrganizationDomainEventAsync(OrganizationDomain organizationDomain, EventType type, DateTime? date = null);
    Task LogOrganizationDomainEventAsync(OrganizationDomain organizationDomain, EventType type, EventSystemUser systemUser, DateTime? date = null);
    Task LogServiceAccountSecretEventAsync(Guid serviceAccountId, Secret secret, EventType type, DateTime? date = null);
    Task LogServiceAccountSecretsEventAsync(Guid serviceAccountId, IEnumerable<Secret> secrets, EventType type, DateTime? date = null);
}
