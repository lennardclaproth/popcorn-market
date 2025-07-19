using Ardalis.GuardClauses;
using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.Errors;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Enums;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;
using PeriodType = Popcorn.FinancialAtlas.Domain.Enums.PeriodType;

namespace PopcornMarket.FinancialAtlas.Application.V1.PublishFinancialStatement;

internal sealed class PublishFinancialStatementCommandHandler : ICommandHandler<PublishFinancialStatementCommand>
{
    private readonly IFinancialStatementRepository _financialStatementRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public PublishFinancialStatementCommandHandler(IFinancialStatementRepository financialStatementRepository, ICompanyRepository companyRepository, IMapper mapper)
    {
        _financialStatementRepository = financialStatementRepository;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the creation of a new financial statement. checks
    /// if a company already exists for that ticker. Gets the current/previous
    /// financial statement if it doesn't exist, yet it publishes it else
    /// it checks if it is after the last published financial statement.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result> Handle(PublishFinancialStatementCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);
        if(company == null) return Result.Failure(CompanyErrors.CompanyNotFound);

        var financialStatement = await _financialStatementRepository.GetMostRecent(request.Ticker, cancellationToken);
        var domainInterval = _mapper.Map<PeriodType>(request.Interval);
        var requestReportingPeriod = new ReportingPeriod(request.Year, domainInterval, request.PeriodNumber);
        
        if (financialStatement != null && financialStatement.ReportingPeriod.CompareTo(requestReportingPeriod) > 0)
        {
            return Result.Failure(FinancialStatementErrors.ReportingPeriodTooSmall);
        }

        var incomeStatement = _mapper.Map<IncomeStatement>(request.IncomeStatement);
        var balanceSheet = _mapper.Map<BalanceSheet>(request.BalanceSheet);
        var cashFlowStatement = _mapper.Map<CashFlowStatement>(request.CashFlowStatement);

        var creationResult = FinancialStatement.Create(
            request.Ticker,
            requestReportingPeriod,
            incomeStatement,
            balanceSheet,
            cashFlowStatement
            );

        if (creationResult.IsFailure)
        {
            return creationResult;
        }
        
        Guard.Against.Null(creationResult.Value, "FinancialStatement.CreationResult");
        
        await _financialStatementRepository.Add(creationResult.Value);
        return Result.Success();
    }
}
