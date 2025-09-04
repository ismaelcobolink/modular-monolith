namespace Evently.Api.Extensions;

internal static class ConfigurationExtension
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (string module in modules)
        {
                configurationBuilder.AddJsonFile($"mdoules.{module}.json", false, true);
                configurationBuilder.AddJsonFile($"mdoules.{module}.Development.json", true, true);
        }
    }
}
