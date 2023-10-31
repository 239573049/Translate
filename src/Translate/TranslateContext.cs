using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Translate;

public class TranslateContext : ServiceCollection
{
    private static IServiceProvider _serviceProvider;

    public static TranslateContext CreateContext()
    {
        return new TranslateContext();
    }

    public void Builder()
    {
        _serviceProvider = this.BuildServiceProvider();
    }

    public static T GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    public static IEnumerable<T> GetServices<T>()
    {
        return _serviceProvider.GetServices<T>();
    }

    public static T GetRequiredService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }


    public static T GetKeyedService<T>(string name)
    {
        return _serviceProvider.GetKeyedService<T>(name);
    }

    public static T GetRequiredKeyedService<T>(string name)
    {
        return _serviceProvider.GetRequiredKeyedService<T>(name);
    }

    public static IEnumerable<T> GetKeyedServices<T>(string name)
    {
        return _serviceProvider.GetKeyedServices<T>(name);
    }
}