using System;
using System.Threading.Tasks;

namespace Tamturk {
    public static class IRevokedHashProviderExtensions {
        public static void Revoke(this IRevokedHashProvider revokedHashProvider, string hash, DateTimeOffset? exp = null) {
            if (!revokedHashProvider.TryRevoke(hash, exp)) {
                throw new HashRevokedException();
            }
        }
        
        public static async Task RevokeAsync(this IRevokedHashProvider revokedHashProvider, string hash, DateTimeOffset? exp = null) {
            if (!await revokedHashProvider.TryRevokeAsync(hash, exp)) {
                throw new HashRevokedException();
            }
        }
        
        public static void ThrowIfRevoked(this IRevokedHashProvider revokedHashProvider, string hash) {
            if (revokedHashProvider.IsRevoked(hash)) {
                throw new HashRevokedException();
            }
        }
        
        public static async Task ThrowIfRevokedAsync(this IRevokedHashProvider revokedHashProvider, string hash) {
            if (await revokedHashProvider.IsRevokedAsync(hash)) {
                throw new HashRevokedException();
            }
        }
    }
}