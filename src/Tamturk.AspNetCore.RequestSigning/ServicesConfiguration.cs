using Microsoft.Extensions.DependencyInjection;

namespace Tamturk.AspNetCore {
    public static class ServicesConfiguration {
        public static IServiceCollection AddRequestSigning
            (this IServiceCollection serviceCollection, string hash) =>
            serviceCollection.AddSingleton(new RequestSigning(hash));
        
        public static IServiceCollection AddInMemoryRevokedHashTable
            (this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton<IRevokedHashProvider, InMemoryRevokedHashProvider>(a => new InMemoryRevokedHashProvider());
    }
}