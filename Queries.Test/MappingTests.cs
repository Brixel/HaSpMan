using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using Queries.MapperProfiles;

using Xunit;

namespace Queries.Test;

public class MappingTests
{
    private readonly IMapper _mapper;

    public MappingTests()
    {
        var mockedLoggerFactory = new Mock<ILoggerFactory>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MemberProfile>();
            cfg.AddProfile<TransactionProfile>();
        }, mockedLoggerFactory.Object);

        _mapper = new Mapper(config);
    }

    [Fact]
    public void AutoMapperAssertions()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
