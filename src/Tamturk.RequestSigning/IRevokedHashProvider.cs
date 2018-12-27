using System;
using System.Threading.Tasks;

namespace Tamturk {
    public interface IRevokedHashProvider {
        bool TryRevoke(string hash, DateTimeOffset? exp);
        
        Task<bool> TryRevokeAsync(string hash, DateTimeOffset? exp);

        bool IsRevoked(string hash);
        
        Task<bool> IsRevokedAsync(string hash);
    }
}