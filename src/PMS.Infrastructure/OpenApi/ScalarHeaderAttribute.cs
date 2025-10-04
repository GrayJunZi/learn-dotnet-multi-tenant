namespace PMS.Infrastructure.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ScalarHeaderAttribute(string HeaderName, string Description, string DefaultValue, bool IsRequired) : Attribute
{

}
