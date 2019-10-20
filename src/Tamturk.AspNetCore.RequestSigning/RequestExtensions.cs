using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tamturk.AspNetCore {
    public static class RequestExtensions {
        public static void ValidateRequest(this HttpRequest request) {
            request.HttpContext.RequestServices.GetRequiredService<IRequestSigning>()
                   .ValidateRequest(request);
        }
        
        public static bool TryValidateRequest(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRequestSigning>()
                   .TryValidateRequest(request);
        }
        
        public static void Revoke(this HttpRequest request) {
            request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                   .Revoke(request);
        }
        
        public static Task RevokeAsync(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                    .RevokeAsync(request);
        }
        
        public static bool TryRevoke(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                    .TryRevoke(request);
        }
        
        public static Task<bool> TryRevokeAsync(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                    .TryRevokeAsync(request);
        }
        
        public static bool IsRevoked(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                          .IsRevoked(request);
        }
        
        public static Task<bool> IsRevokedAsync(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                          .IsRevokedAsync(request);
        }
        
        public static void ThrowIfRevoked(this HttpRequest request) {
            request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                          .ThrowIfRevoked(request);
        }
        
        public static Task ThrowIfRevokedAsync(this HttpRequest request) {
            return request.HttpContext.RequestServices.GetRequiredService<IRevokedHashProvider>()
                          .ThrowIfRevokedAsync(request);
        }
    }
}