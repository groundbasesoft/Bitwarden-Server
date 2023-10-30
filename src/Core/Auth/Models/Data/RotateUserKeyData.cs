using Bit.Core.Auth.Entities;
using Bit.Core.Entities;
using Bit.Core.Tools.Entities;
using Bit.Core.Vault.Entities;

public class RotateUserKeyData
{
    public User User { get; set; }
    public string MasterPasswordHash { get; set; }
    public string Key { get; set; }
    public string PrivateKey { get; set; }
    public IEnumerable<Cipher> Ciphers { get; set; }
    public IEnumerable<Folder> Folders { get; set; }
    public IEnumerable<Send> Sends { get; set; }
    public IEnumerable<EmergencyAccess> EmergencyAccessKeys { get; set; }
    public IEnumerable<OrganizationUser> AccountRecoveryKeys { get; set; }
}
