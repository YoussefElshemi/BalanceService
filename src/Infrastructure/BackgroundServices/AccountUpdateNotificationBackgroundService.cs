using Core.Configs;
using Core.Dtos;
using Core.Models;
using Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class AccountUpdateNotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<AccountUpdateNotificationConfig> configOptions)
    : UpdateNotificationBackgroundService<
        AccountHistoryEntity,
        AccountHistory,
        HistoryDto<AccountHistoryDto>,
        AccountUpdateNotificationConfig>(
        scopeFactory,
        configOptions);
