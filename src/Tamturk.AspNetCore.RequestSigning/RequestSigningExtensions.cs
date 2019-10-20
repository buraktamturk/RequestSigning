using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Tamturk.AspNetCore {
    public static class IRequestSigningExtensions {
        public static void ValidateRequest(this IRequestSigning requestSigning, HttpRequest request) {
            requestSigning.ValidateRequest(request.Method, request.Path, request.Query.ToDictionary(x => x.Key, y => y.Value.ToString()));
        }
        
        public static bool TryValidateRequest(this IRequestSigning requestSigning, HttpRequest request) {
            return requestSigning.TryValidateRequest(request.Method, request.Path, request.Query.ToDictionary(x => x.Key, y => y.Value.ToString()));
        }
        
        public static void Revoke(this IRevokedHashProvider revokedHashs, HttpRequest request) {
            revokedHashs.Revoke(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static Task RevokeAsync(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.RevokeAsync(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static bool TryRevoke(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.TryRevoke(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static Task<bool> TryRevokeAsync(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.TryRevokeAsync(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static bool IsRevoked(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.IsRevoked(request.Query["sig"]);
        }
        
        public static Task<bool> IsRevokedAsync(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.IsRevokedAsync(request.Query["sig"]);
        }
        
        public static void ThrowIfRevoked(this IRevokedHashProvider requestSigning, HttpRequest request) {
            requestSigning.ThrowIfRevoked(request.Query["sig"]);
        }
        
        public static Task ThrowIfRevokedAsync(this IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.ThrowIfRevokedAsync(request.Query["sig"]);
        }
    }
}