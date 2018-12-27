using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Tamturk.AspNetCore {
    public static class RequestSigningExtensions {
        public static void ValidateRequest(this Tamturk.RequestSigning requestSigning, HttpRequest request) {
            requestSigning.ValidateRequest(request.Method, request.Path, request.Query.ToDictionary(x => x.Key, y => y.Value.ToString()));
        }
        
        public static bool TryValidateRequest(this Tamturk.RequestSigning requestSigning, HttpRequest request) {
            return requestSigning.TryValidateRequest(request.Method, request.Path, request.Query.ToDictionary(x => x.Key, y => y.Value.ToString()));
        }
        
        public static void Revoke(this Tamturk.IRevokedHashProvider revokedHashs, HttpRequest request) {
            revokedHashs.Revoke(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? Tamturk.RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static Task RevokeAsync(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.RevokeAsync(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? Tamturk.RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static bool TryRevoke(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.TryRevoke(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? Tamturk.RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static Task<bool> TryRevokeAsync(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.TryRevokeAsync(request.Query["sig"], request.Query.TryGetValue("exp", out StringValues exp) ? Tamturk.RequestSigning.UnixTimeStampToDateTime(long.Parse(exp.ToString())) : null);
        }
        
        public static bool IsRevoked(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.IsRevoked(request.Query["sig"]);
        }
        
        public static Task<bool> IsRevokedAsync(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.IsRevokedAsync(request.Query["sig"]);
        }
        
        public static void ThrowIfRevoked(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            requestSigning.ThrowIfRevoked(request.Query["sig"]);
        }
        
        public static Task ThrowIfRevokedAsync(this Tamturk.IRevokedHashProvider requestSigning, HttpRequest request) {
            return requestSigning.ThrowIfRevokedAsync(request.Query["sig"]);
        }
    }
}