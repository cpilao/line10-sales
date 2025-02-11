namespace Line10.Sales.IntegrationTests.Api;

[Collection("IntegrationApiTestCollection")]
public abstract class BaseApiIntegrationTest
{
    protected readonly IntegrationApiTestFixture _fixture;

    protected BaseApiIntegrationTest(IntegrationApiTestFixture fixture)
    {
        _fixture = fixture;
    }
}