using System.Reflection;

namespace Xunit;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FactAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TheoryAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InlineDataAttribute(params object?[] data) : Attribute
{
    public object?[] Data { get; } = data;
}

public static class Assert
{
    public static void True(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new Exception(message ?? "Expected condition to be true.");
        }
    }

    public static void False(bool condition, string? message = null) => True(!condition, message ?? "Expected condition to be false.");

    public static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new Exception($"Expected '{expected}' but found '{actual}'.");
        }
    }

    public static void NotNull(object? value)
    {
        if (value is null)
        {
            throw new Exception("Expected value to be not null.");
        }
    }

    public static void Empty<T>(IEnumerable<T> collection)
    {
        if (collection.Any())
        {
            throw new Exception("Expected collection to be empty.");
        }
    }

    public static void Single<T>(IEnumerable<T> collection)
    {
        if (collection.Count() != 1)
        {
            throw new Exception($"Expected a single item, but found {collection.Count()}.");
        }
    }

    public static void Collection<T>(IEnumerable<T> collection, params Action<T>[] inspectors)
    {
        var items = collection.ToArray();
        if (items.Length != inspectors.Length)
        {
            throw new Exception($"Expected {inspectors.Length} items, but found {items.Length}.");
        }

        for (var index = 0; index < items.Length; index++)
        {
            inspectors[index](items[index]);
        }
    }
}

public static class TestRunner
{
    public static async Task<int> RunAsync(Assembly assembly)
    {
        var failures = new List<string>();
        foreach (var type in assembly.GetTypes().Where(type => type is { IsAbstract: false, IsGenericTypeDefinition: false }))
        {
            var testMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(method => method.GetCustomAttribute<FactAttribute>() is not null || method.GetCustomAttribute<TheoryAttribute>() is not null)
                .ToArray();

            if (testMethods.Length == 0)
            {
                continue;
            }

            var instance = Activator.CreateInstance(type);
            if (instance is null)
            {
                failures.Add($"{type.FullName}: could not create test instance.");
                continue;
            }

            foreach (var method in testMethods)
            {
                var fact = method.GetCustomAttribute<FactAttribute>();
                if (fact is not null)
                {
                    await InvokeTestMethodAsync(instance, method, Array.Empty<object?>(), failures).ConfigureAwait(false);
                    continue;
                }

                var inlineData = method.GetCustomAttributes<InlineDataAttribute>().ToArray();
                if (inlineData.Length == 0)
                {
                    failures.Add($"{type.FullName}.{method.Name}: theory without InlineData.");
                    continue;
                }

                foreach (var data in inlineData)
                {
                    await InvokeTestMethodAsync(instance, method, data.Data, failures).ConfigureAwait(false);
                }
            }
        }

        if (failures.Count > 0)
        {
            foreach (var failure in failures)
            {
                Console.Error.WriteLine(failure);
            }

            return 1;
        }

        Console.WriteLine("All tests passed.");
        return 0;
    }

    private static async Task InvokeTestMethodAsync(object instance, MethodInfo method, object?[] arguments, List<string> failures)
    {
        try
        {
            var result = method.Invoke(instance, arguments);
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            failures.Add($"{method.DeclaringType?.FullName}.{method.Name}: {ex.InnerException.Message}");
        }
        catch (Exception ex)
        {
            failures.Add($"{method.DeclaringType?.FullName}.{method.Name}: {ex.Message}");
        }
    }
}
