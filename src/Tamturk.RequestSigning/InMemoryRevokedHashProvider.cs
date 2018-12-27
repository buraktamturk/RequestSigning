using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Tamturk {
    public class InMemoryRevokedHashProvider : IRevokedHashProvider {
        private ConcurrentDictionary<byte[], DateTimeOffset?> cache = new ConcurrentDictionary<byte[], DateTimeOffset?>();

        public int cleanupInterval = 1000;
        public int tries = 0;
        
        public bool TryRevoke(string hash, DateTimeOffset? exp = null) {
            byte[] _hash = Enumerable.Range(0, hash.Length)
                                     .Where(x => x % 2 == 0)
                                     .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
                                     .ToArray();
            
            if (++tries == cleanupInterval) {
                DateTimeOffset time = DateTimeOffset.UtcNow;
                cache = new ConcurrentDictionary<byte[], DateTimeOffset?>(cache.Where(a => a.Value >= time));
            }

            if (!cache.TryAdd(_hash, exp)) {
                return false;
            }
            
            return true;
        }

        public Task<bool> TryRevokeAsync(string hash, DateTimeOffset? exp = null) {
            return Task.FromResult(TryRevoke(hash, exp));
        }

        public bool IsRevoked(string hash) {
            return cache.ContainsKey(Enumerable.Range(0, hash.Length)
               .Where(x => x % 2 == 0)
               .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
               .ToArray());
        }
        
        public Task<bool> IsRevokedAsync(string hash) {
            return Task.FromResult(IsRevoked(hash));
        }
    }
}