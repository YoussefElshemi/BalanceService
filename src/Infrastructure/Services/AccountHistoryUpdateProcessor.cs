using Amazon.SimpleNotificationService;
using Core.Configs;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class AccountHistoryUpdateProcessor(
    IAmazonSimpleNotificationService amazonSns,
    IHistoryRepository<AccountHistoryEntity, AccountHistory> accountHistoryRepository,
    IUnitOfWork unitOfWork,
    IOptions<AccountUpdateNotificationConfig> configOptions)
    : HistoryUpdateProcessor<
        AccountHistoryEntity,
        AccountHistory,
        HistoryDto<AccountHistoryDto>,
        AccountUpdateNotificationConfig>(
        amazonSns,
        accountHistoryRepository,
        unitOfWork,
        configOptions);
