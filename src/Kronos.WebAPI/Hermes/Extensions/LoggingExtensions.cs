namespace Kronos.WebAPI.Hermes.Extensions;

public static class LoggingExtensions
{
    public static void LogHermesApiError(this ILogger logger, string methodName, Enum error)
    {
        logger.LogError("HERMES API - operation {OperationName} failed with error: {Error}",
            methodName, error);
    }
    
    public static void LogAthenaApiError(this ILogger logger, string methodName, Enum error)
    {
        logger.LogError("ATHENA API - operation {OperationName} failed with error: {Error}",
            methodName, error);
    }
    
    public static void LogAthenaAdminApiError(this ILogger logger, string methodName, Enum error)
    {
        logger.LogError("ATHENA ADMIN API - operation {OperationName} failed with error: {Error}",
            methodName, error);
    }
    
    public static void LogHermesAdminApiError(this ILogger logger, string methodName, Enum error)
    {
        logger.LogError("HERMES ADMIN API - operation {OperationName} failed with error: {Error}",
            methodName, error);
    }
}