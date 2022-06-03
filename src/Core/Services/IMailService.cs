﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Entities;
using Bit.Core.Models.Business;
using Bit.Core.Models.Mail;

namespace Bit.Core.Services
{
    public interface IMailService
    {
        Task SendWelcomeEmailAsync(User user);
        Task SendVerifyEmailEmailAsync(string email, Guid userId, string token);
        Task SendVerifyDeleteEmailAsync(string email, Guid userId, string token);
        Task SendChangeEmailAlreadyExistsEmailAsync(string fromEmail, string toEmail);
        Task SendChangeEmailEmailAsync(string newEmailAddress, string token);
        Task SendTwoFactorEmailAsync(string email, string token);
        Task SendNewDeviceLoginTwoFactorEmailAsync(string email, string token);
        Task SendNoMasterPasswordHintEmailAsync(string email);
        Task SendMasterPasswordHintEmailAsync(string email, string hint);
        Task SendOrganizationInviteEmailAsync(string organizationName, OrganizationUser orgUser, ExpiringToken token);
        Task BulkSendOrganizationInviteEmailAsync(string organizationName, IEnumerable<(OrganizationUser orgUser, ExpiringToken token)> invites);
        Task SendOrganizationMaxSeatLimitReachedEmailAsync(Organization organization, int maxSeatCount, IEnumerable<string> ownerEmails);
        Task SendOrganizationAutoscaledEmailAsync(Organization organization, int initialSeatCount, IEnumerable<string> ownerEmails);
        Task SendOrganizationAcceptedEmailAsync(Organization organization, string userIdentifier, IEnumerable<string> adminEmails);
        Task SendOrganizationConfirmedEmailAsync(string organizationName, string email);
        Task SendOrganizationUserRemovedForPolicyTwoStepEmailAsync(string organizationName, string email);
        Task SendPasswordlessSignInAsync(string returnUrl, string token, string email);
        Task SendInvoiceUpcomingAsync(string email, decimal amount, DateTime dueDate, List<string> items,
            bool mentionInvoices);
        Task SendPaymentFailedAsync(string email, decimal amount, bool mentionInvoices);
        Task SendAddedCreditAsync(string email, decimal amount);
        Task SendLicenseExpiredAsync(IEnumerable<string> emails, string organizationName = null);
        Task SendNewDeviceLoggedInEmail(string email, string deviceType, DateTime timestamp, string ip);
        Task SendRecoverTwoFactorEmail(string email, DateTime timestamp, string ip);
        Task SendOrganizationUserRemovedForPolicySingleOrgEmailAsync(string organizationName, string email);
        Task SendEmergencyAccessInviteEmailAsync(EmergencyAccess emergencyAccess, string name, string token);
        Task SendEmergencyAccessAcceptedEmailAsync(string granteeEmail, string email);
        Task SendEmergencyAccessConfirmedEmailAsync(string grantorName, string email);
        Task SendEmergencyAccessRecoveryInitiated(EmergencyAccess emergencyAccess, string initiatingName, string email);
        Task SendEmergencyAccessRecoveryApproved(EmergencyAccess emergencyAccess, string approvingName, string email);
        Task SendEmergencyAccessRecoveryRejected(EmergencyAccess emergencyAccess, string rejectingName, string email);
        Task SendEmergencyAccessRecoveryReminder(EmergencyAccess emergencyAccess, string initiatingName, string email);
        Task SendEmergencyAccessRecoveryTimedOut(EmergencyAccess ea, string initiatingName, string email);
        Task SendEnqueuedMailMessageAsync(IMailQueueMessage queueMessage);
        Task SendAdminResetPasswordEmailAsync(string email, string userName, string orgName);
        Task SendUpdatedTempPasswordEmailAsync(string email, string userName);
        Task SendOTPEmailAsync(string email, string token);
        Task SendFailedLoginAttemptsEmailAsync(string email, DateTime utcNow, string ip);
        Task SendFailedTwoFactorAttemptsEmailAsync(string email, DateTime utcNow, string ip);
        Task EnqueueMailAsync<T>(MailMessage message, string templateName, T model);
        Task EnqueueMailAsync(IEnumerable<IMailQueueMessage> messages);
    }
}
