using AutoMapper;

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
        }, null);

        _mapper = new Mapper(config);
    }

    [Fact]
    public void AutoMapperAssertions()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
