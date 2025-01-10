﻿namespace Services.Catalog.Extensions;

internal static class Extensions
{
    internal static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddDefaultAuthentication();

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddHttpContextAccessor();

        builder.AddDateTimeProvider();

        builder.AddConfigureIdentity();

        builder.AddConfigureFastEndpoints();

        builder.AddPersistence();

        builder.Services.AddMediatR(
            cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<ICatalogAssemblyMaker>();
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(MetricsBehavior<,>));
            });

        builder.Services.AddSingleton<IActivityScope, ActivityScope>();
        builder.Services.AddSingleton<CommandHandlerMetrics>();
        builder.Services.AddSingleton<QueryHandlerMetrics>();

        return builder;
    }

    internal static IApplicationBuilder UseApplicationServices(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseDefaultExceptionHandler()
           .UseFastEndpoints(
               c =>
               {
                   c.Endpoints.RoutePrefix = "api";
                   c.Versioning.Prefix = "v";
                   c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                   c.Errors.UseProblemDetails();
               });

        app.UseStatusCodePages();

        return app;
    }
}