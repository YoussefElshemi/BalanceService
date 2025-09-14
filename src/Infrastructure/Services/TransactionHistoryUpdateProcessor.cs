using Amazon.SimpleNotificationService;
using Core.Configs;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities.History;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class TransactionHistoryUpdateProcessor(
    IAmazonSimpleNotificationService amazonSns,
    IHistoryRepository<TransactionHistoryEntity, TransactionHistory> transactionHistoryRepository,
    IUnitOfWork unitOfWork,
    IOptions<TransactionUpdateNotificationConfig> configOptions)
    : HistoryUpdateProcessor<
        TransactionHistoryEntity,
        TransactionHistory,
        HistoryDto<TransactionHistoryDto>,
        TransactionUpdateNotificationConfig>(
        amazonSns,
        transactionHistoryRepository,
        unitOfWork,
        configOptions);
