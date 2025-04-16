using BStorm.Tools.CommandQuerySeparation.Commands;
using BStorm.Tools.CommandQuerySeparation.Dispatching;
using BStorm.Tools.CommandQuerySeparation.Queries;
using System.Reflection;

namespace RefreshTokenSample.Api.Infrastructure
{
    public static class IServiceCollectionExtensions
    {
        private static readonly Type[] validTypes = [typeof(ICommandHandler<>), typeof(ICommandAsyncHandler<>), typeof(IQueryHandler<,>), typeof(IQueryAsyncHandler<,>)];
        
        public static IServiceCollection AddCqsHandlersAndDispatcher(this IServiceCollection services)
        {          
            Assembly entryAssembly = Assembly.GetEntryAssembly()!;
            Type[] handlers = entryAssembly!.GetTypes()
                .Union(entryAssembly.GetReferencedAssemblies().SelectMany(an => Assembly.Load(an).GetTypes()))
                .Where(t => t.GetInterfaces().Any(IsHandlerInterface) && t.Name.EndsWith("Handler"))
                .ToArray();

            foreach (Type handlerType in handlers)
            {
                Type @interface = GetInterface(handlerType);

                services.AddScoped(@interface, handlerType);
            }

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        private static Type GetInterface(Type handlerType)
        {
            return handlerType.GetInterfaces().Single(IsHandlerInterface);
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return validTypes.Contains(typeDefinition);
        }
    }
}
