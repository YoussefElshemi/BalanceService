using System.Reflection;
using Amazon.SimpleNotificationService;
using Core.Configs;
using Core.Interfaces;
using Core.Services;
using FluentValidation;
using Infrastructure;
using Infrastructure.BackgroundServices;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Presentation.ExceptionHandlers;

using AccountUpdateBackgroundService = Infrastructure.BackgroundServices.UpdateNotificationBackgroundService<
    Infrastructure.Entities.History.AccountHistoryEntity,
    Core.Models.AccountHistory,
    System.Guid,
    Core.Configs.AccountUpdateNotificationConfig,
    Core.Dtos.HistoryDto<Core.Dtos.AccountHistoryDto>>;

using TransactionUpdateBackgroundService = Infrastructure.BackgroundServices.UpdateNotificationBackgroundService<
    Infrastructure.Entities.History.TransactionHistoryEntity,
    Core.Models.TransactionHistory,
    System.Guid,
    Core.Configs.TransactionUpdateNotificationConfig,
    Core.Dtos.HistoryDto<Core.Dtos.TransactionHistoryDto>>;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterConfigurations(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var config = configuration.AddEnvironmentVariables().Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddOptions<AppConfig>().BindConfiguration(nameof(AppConfig));


        return services;
    }

    public static IServiceCollection RegisterBackgroundServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<AccountUpdateNotificationConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(AccountUpdateNotificationConfig)));
        services.Configure<TransactionUpdateNotificationConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(TransactionUpdateNotificationConfig)));

        services
            .AddHostedService<HoldExpiryBackgroundService>()
            .AddHostedService<AccountUpdateBackgroundService>()
            .AddHostedService<TransactionUpdateBackgroundService>();

        return services;
    }

    public static IServiceCollection RegisterAwsServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonSimpleNotificationService>();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services
            .AddSingleton(TimeProvider.System)
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IAccountRulesService, AccountRulesService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<ITransferService, TransferService>()
            .AddScoped<IStatementService, StatementService>()
            .AddScoped<IHoldService, HoldService>()
            .AddScoped(typeof(IHistoryUpdateProcessor<,,,,>), typeof(HistoryUpdateProcessor<,,,,>));

        return services;
    }

    public static IServiceCollection RegisterDataAccess(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString(ApplicationDbContext.DatabaseConnectionName),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.EnableRetryOnFailure();
                });

            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });

        services
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddScoped<IHoldRepository, HoldRepository>()
            .AddScoped<IStatementRepository, StatementRepository>()
            .AddScoped(typeof(IHistoryRepository<,,>), typeof(HistoryRepository<,,>))
            .AddScoped<IUnitOfWork>(x => x.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    public static IServiceCollection RegisterExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static IServiceCollection RegisterHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        return services;
    }

    public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ABOR",
                Version = "v1"
            });
        });

        return services;
    }
}