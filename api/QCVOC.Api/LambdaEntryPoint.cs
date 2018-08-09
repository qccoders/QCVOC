namespace QCVOC.Api
{
    using Amazon.Lambda.AspNetCoreServer;
    using Microsoft.AspNetCore.Hosting;

    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>()
                .UseApiGateway();
        }
    }
}