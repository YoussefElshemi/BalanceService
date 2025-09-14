using Amazon.SimpleNotificationService;
using Core.Configs;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities.History;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class HoldHistoryUpdateProcessor(
    IAmazonSimpleNotificationService amazonSns,
    IHistoryRepository<HoldHistoryEntity, HoldHistory> holdHistoryRepositor,
    IUnitOfWork unitOfWork,
    IOptions<HoldUpdateNotificationConfig> configOptions)
    : HistoryUpdateProcessor<
        HoldHistoryEntity,
        HoldHistory,
        HistoryDto<HoldHistoryDto>,
        HoldUpdateNotificationConfig>(
        amazonSns,
        holdHistoryRepositor,
        unitOfWork,
        configOptions);
