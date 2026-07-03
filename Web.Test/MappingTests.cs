using AutoMapper;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Web.MapperProfiles;

using Xunit;

namespace Web.Test;

public class MappingTests
{
    private readonly IMapper _mapper;
    public MappingTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MemberProfile>();
            cfg.AddProfile<TransactionProfile>();
            cfg.AddProfile<BankAccountProfile>();
        }, NullLoggerFactory.Instance);

        _mapper = new Mapper(config);
    }

    [Fact]
    public void AutoMapperAssertions()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
