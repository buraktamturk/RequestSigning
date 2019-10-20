using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tamturk.AspNetCore {
    public static class ServicesConfiguration {
        public static IServiceCollection AddRequestSigning
            (this IServiceCollection serviceCollection, string hash, Action<IRequestSigning> threadSafeInstance = null) {
            threadSafeInstance?.Invoke(new ThreadSafeRequestSigning(hash));
            return serviceCollection.AddScoped<IRequestSigning, RequestSigning>(a => new RequestSigning(hash));
        }

        public static IServiceCollection AddInMemoryRevokedHashTable
            (this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton<IRevokedHashProvider, InMemoryRevokedHashProvider>(a => new InMemoryRevokedHashProvider());
    }
}