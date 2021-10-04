﻿using System;
using System.Collections.Generic;

using MediatR;

using Types;

namespace Commands.Handlers.Transaction.AddDebitAcquisitionConsumables
{
    public record AddDebitAcquisitionConsumablesCommand(CounterParty CounterParty, BankAccount BankAccount,
        decimal Amount,
        DateTime ReceivedDateTime, string Description, int Sequence,
        ICollection<Domain.Transaction.TransactionAttachment> Attachments) : IRequest<Guid>;
}