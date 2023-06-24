﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Commands.Handlers.FinancialYear.CloseFinancialYear;

using Domain;

using FluentAssertions;

using Moq;

using Persistence.Repositories;

using Xunit;

namespace Commands.Test.FinancialYear;

public class When_closing_a_financial_year
{
    private readonly Mock<IFinancialYearRepository> _financialYearRepositoryMock;

    public When_closing_a_financial_year()
    {
        _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
        SUT = new CloseFinancialYearHandler(_financialYearRepositoryMock.Object);
    }

    public CloseFinancialYearHandler SUT { get; set; }

    [Fact]
    public async Task It_should_mark_the_year_as_closed()
    {
        var financialYear = new Domain.FinancialYear(DateOnly.FromDateTime(DateTime.Now),
            new List<Domain.Transaction>());
        _financialYearRepositoryMock
            .Setup(x => x.GetById(financialYear.Id, CancellationToken.None))
            .ReturnsAsync(financialYear);
;       await SUT.Handle(new CloseFinancialYearCommand(financialYear.Id), CancellationToken.None);
        
        financialYear.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task It_should_mark_all_related_transactions_as_as_locked()
    {
        var financialYear = new Domain.FinancialYear(DateOnly.FromDateTime(DateTime.Now),
            new List<Domain.Transaction>()
            {
                new CreditTransaction("Random counter party", Guid.NewGuid(), 20,DateTimeOffset.Now, "A description", new List<TransactionAttachment>(), null, new List<TransactionTypeAmount>())
            });
        _financialYearRepositoryMock
            .Setup(x => x.GetById(financialYear.Id, CancellationToken.None))
            .ReturnsAsync(financialYear);
        ;
        await SUT.Handle(new CloseFinancialYearCommand(financialYear.Id), CancellationToken.None);

        financialYear.IsClosed.Should().BeTrue();
        financialYear.Transactions.Should().OnlyContain(x => x.Locked);
    }

    [Fact]
    public async Task It_should_throw_an_exception_when_no_year_is_found()
    {
        var newGuid = Guid.NewGuid();
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
       {
           await SUT.Handle(new CloseFinancialYearCommand(newGuid), CancellationToken.None);
       });
        Assert.Equal($"No financial year found by Id {newGuid} (Parameter 'Id')", exception.Message);
    }
}