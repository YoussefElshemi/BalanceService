using Core.Configs;
using Core.Dtos;
using Core.Models;
using Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class TransactionUpdateNotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<TransactionUpdateNotificationConfig> configOptions)
    : UpdateNotificationBackgroundService<
        TransactionHistoryEntity,
        TransactionHistory,
        HistoryDto<TransactionHistoryDto>,
        TransactionUpdateNotificationConfig>(
        scopeFactory,
        configOptions);
