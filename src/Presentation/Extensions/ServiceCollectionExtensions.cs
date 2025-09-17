using System.Reflection;
using Amazon.SimpleNotificationService;
using Core.Configs;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using FluentValidation;
using Infrastructure;
using Infrastructure.BackgroundServices;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Presentation.ExceptionHandlers;

using IAccountHistoryUpdateProcessor = Infrastructure.Services.Interfaces.IHistoryUpdateProcessor<
    Infrastructure.Entities.History.AccountHistoryEntity,
    Core.Models.AccountHistory,
    Core.Dtos.HistoryDto<Core.Dtos.AccountHistoryDto>,
    Core.Configs.AccountUpdateNotificationConfig>;

using ITransactionHistoryUpdateProcessor = Infrastructure.Services.Interfaces.IHistoryUpdateProcessor<
    Infrastructure.Entities.History.TransactionHistoryEntity,
    Core.Models.TransactionHistory,
    Core.Dtos.HistoryDto<Core.Dtos.TransactionHistoryDto>,
    Core.Configs.TransactionUpdateNotificationConfig>;

using IHoldHistoryUpdateProcessor = Infrastructure.Services.Interfaces.IHistoryUpdateProcessor<
    Infrastructure.Entities.History.HoldHistoryEntity,
    Core.Models.HoldHistory,
    Core.Dtos.HistoryDto<Core.Dtos.HoldHistoryDto>,
    Core.Configs.HoldUpdateNotificationConfig>;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterConfigurations(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var config = configuration.AddEnvironmentVariables().Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddOptions<AppConfig>().BindConfiguration(nameof(AppConfig));

        return services;
    }

    public static IServiceCollection RegisterBackgroundServices(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        services.Configure<AccountUpdateNotificationConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(AccountUpdateNotificationConfig)));
        services.Configure<TransactionUpdateNotificationConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(TransactionUpdateNotificationConfig)));
        services.Configure<HoldUpdateNotificationConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(HoldUpdateNotificationConfig)));

        services
            .AddHostedService<HoldExpiryBackgroundService>()
            .AddHostedService<AccountUpdateNotificationBackgroundService>()
            .AddHostedService<TransactionUpdateNotificationBackgroundService>()
            .AddHostedService<HoldUpdateNotificationBackgroundService>();

        return services;
    }

    public static IServiceCollection RegisterJobs(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<InterestAccrualJobConfig>(
            configuration.GetSection(nameof(AppConfig)).GetSection(nameof(InterestAccrualJobConfig)));

        services
            .AddScoped<InterestAccrualService>()
            .AddHostedService<JobBackgroundService<InterestAccrualJobConfig, InterestAccrualService>>();

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
            .AddSingleton<ICurrencyService, CurrencyService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IAccountRulesService, AccountRulesService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<ITransferService, TransferService>()
            .AddScoped<IStatementService, StatementService>()
            .AddScoped<IBalanceService, BalanceService>()
            .AddScoped<IHoldService, HoldService>()
            .AddScoped<IInterestProductAccountLinkService, InterestProductAccountLinkService>()
            .AddScoped<IJobService, JobService>()
            .AddScoped<IHistoryService<AccountHistory>, AccountHistoryService>()
            .AddScoped<IHistoryService<TransactionHistory>, TransactionHistoryService>()
            .AddScoped<IHistoryService<HoldHistory>, HoldHistoryService>()
            .AddScoped<IAccountHistoryUpdateProcessor, AccountHistoryUpdateProcessor>()
            .AddScoped<ITransactionHistoryUpdateProcessor, TransactionHistoryUpdateProcessor>()
            .AddScoped<IHoldHistoryUpdateProcessor, HoldHistoryUpdateProcessor>();

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
            .AddScoped<IBalanceRepository, BalanceRepository>()
            .AddScoped<IInterestAccrualRepository, InterestAccrualRepository>()
            .AddScoped<IInterestProductAccountLinkRepository, InterestProductAccountLinkRepository>()
            .AddScoped<IJobRepository, JobRepository>()
            .AddScoped<IJobRunRepository, JobRunRepository>()
            .AddScoped(typeof(IHistoryRepository<,>), typeof(HistoryRepository<,>))
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
                Title = "Balance Service",
                Version = "v1"
            });
        });

        return services;
    }
}