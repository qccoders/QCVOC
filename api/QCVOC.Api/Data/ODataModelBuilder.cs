namespace QCVOC.Api.Data
{
    using System;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.OData.Edm;
    using QCVOC.Api.Data.DTO;

    public class ODataModelBuilder
    {
        public IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);

            builder.EntitySet<AccountResponse>(nameof(AccountResponse))
                .EntityType
                .Filter()
                .Count()
                .OrderBy()
                .Page()
                .Select();

            return builder.GetEdmModel();
        }
    }
}
