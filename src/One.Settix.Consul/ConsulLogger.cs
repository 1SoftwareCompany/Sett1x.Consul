using System;
using System.Reflection;

namespace One.Settix.Consul;

internal static class ConsulLogger
{
    private enum Level
    {
        Information,
        Warning,
        Error
    }

    private static readonly object initLock = new object();
    private static PropertyInfo loggerProperty;
    private static MethodInfo forContextMethod;
    private static MethodInfo infoMethod;
    private static MethodInfo warningMethod;
    private static MethodInfo errorMethod;

    public static void Information(Type source, string messageTemplate, params object[] propertyValues)
    {
        if (TryLog(source, Level.Information, [messageTemplate, propertyValues]))
            return;

        Console.WriteLine(FormatFallback(messageTemplate, propertyValues));
    }

    public static void Warning(Type source, string messageTemplate, params object[] propertyValues)
    {
        if (TryLog(source, Level.Warning, [messageTemplate, propertyValues]))
            return;

        Console.WriteLine(FormatFallback(messageTemplate, propertyValues));
    }

    public static void Error(Type source, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        if (TryLog(source, Level.Error, [exception, messageTemplate, propertyValues]))
            return;

        Console.WriteLine($"{FormatFallback(messageTemplate, propertyValues)}{Environment.NewLine}{exception?.Message}");
    }

    private static bool TryLog(Type source, Level level, object[] args)
    {
        EnsureSerilogResolved();

        MethodInfo logMethod = level switch
        {
            Level.Information => infoMethod,
            Level.Warning => warningMethod,
            Level.Error => errorMethod,
            _ => null
        };

        if (logMethod is null)
            return false;

        try
        {
            object currentLogger = loggerProperty.GetValue(null);
            if (currentLogger is null || currentLogger.GetType().Name == "SilentLogger")
                return false;

            object scopedLogger = forContextMethod?.Invoke(null, [source]) ?? currentLogger;
            logMethod.Invoke(scopedLogger, args);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void EnsureSerilogResolved()
    {
        if (loggerProperty is not null)
            return;

        lock (initLock)
        {
            if (loggerProperty is not null)
                return;

            Type serilogLogType = Type.GetType("Serilog.Log, Serilog", throwOnError: false);
            Type serilogILoggerType = Type.GetType("Serilog.ILogger, Serilog", throwOnError: false);
            if (serilogLogType is null || serilogILoggerType is null)
                return;

            loggerProperty = serilogLogType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Static);
            forContextMethod = serilogLogType.GetMethod("ForContext", BindingFlags.Public | BindingFlags.Static, [typeof(Type)]);
            infoMethod = serilogILoggerType.GetMethod("Information", BindingFlags.Public | BindingFlags.Instance, [typeof(string), typeof(object[])]);
            warningMethod = serilogILoggerType.GetMethod("Warning", BindingFlags.Public | BindingFlags.Instance, [typeof(string), typeof(object[])]);
            errorMethod = serilogILoggerType.GetMethod("Error", BindingFlags.Public | BindingFlags.Instance, [typeof(Exception), typeof(string), typeof(object[])]);
        }
    }

    private static string FormatFallback(string template, object[] values)
    {
        if (values is null || values.Length == 0)
            return template;

        return $"{template} [{string.Join(", ", values)}]";
    }
}
