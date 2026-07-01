#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.TwentyThree.Api;

public class TwentyThreeSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => TwentyThreeApiConstants.Name;

}