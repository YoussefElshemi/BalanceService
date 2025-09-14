using Core.Configs;
using Core.Dtos;
using Core.Models;
using Infrastructure.Entities.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class HoldUpdateNotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<HoldUpdateNotificationConfig> configOptions)
    : UpdateNotificationBackgroundService<
        HoldHistoryEntity,
        HoldHistory,
        HistoryDto<HoldHistoryDto>,
        HoldUpdateNotificationConfig>(
        scopeFactory,
        configOptions);
