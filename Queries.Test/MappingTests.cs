using AutoMapper;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Queries.MapperProfiles;

using Xunit;

namespace Queries.Test;

public class MappingTests
{
    private readonly IMapper _mapper;

    public MappingTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MemberProfile>();
            cfg.AddProfile<TransactionProfile>();
        }, NullLoggerFactory.Instance);

        _mapper = new Mapper(config);
    }

    [Fact]
    public void AutoMapperAssertions()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
