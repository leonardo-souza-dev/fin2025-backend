namespace Fin.Api.Infrastructure;

public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object value)
    {
        return value?.ToString()?.ToKebabCase();
    }
}
