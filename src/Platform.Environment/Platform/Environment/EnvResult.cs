namespace Hyprx.Platform.Environment;

/// <summary>
/// Represents the result of an environment variable operation.
/// </summary>
public class EnvResult
{
    private readonly string? value;

    private readonly EnvironmentException? exception;

    internal EnvResult(EnvironmentException? exception)
    {
        this.exception = exception;
        this.HasValue = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvResult"/> class.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="value">The value of the environment variable.</param>
    internal EnvResult(string name, string? value)
    {
        this.value = value;
        this.HasValue = !string.IsNullOrEmpty(value);
        if (value is null)
            this.exception = new EnvironmentException($"Environment variable '{name}' is not set.");
    }

    /// <summary>
    /// Gets the value of the environment variable.
    /// </summary>
    /// <exception cref="EnvironmentException">Thrown when the value is not set.</exception>
    public string Value
    {
        get
        {
            if (this.value is null)
            {
#pragma warning disable S2372
                throw new EnvironmentException("Value is not set.");
            }

            return this.value;
        }
    }

    /// <summary>
    /// Gets the exception associated with the environment variable.
    /// </summary>
    /// <exception cref="EnvironmentException">Throw when the exception is not set.</exception>
    public EnvironmentException Exception
    {
        get
        {
            if (this.exception is null)
            {
#pragma warning disable S2372
                throw new EnvironmentException("Exception is not set.");
            }

            return this.exception;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the environment variable has a value.
    /// </summary>
    public bool HasValue { get; }

    public static implicit operator string(EnvResult result)
        => result.Value;

    public static implicit operator EnvResult(string value)
        => new(string.Empty, value);

    /// <summary>
    /// Returns the value of the environment variable or throws an exception with the specified message if the value is not set.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <returns>The value of the environment variable.</returns>
    /// <exception cref="EnvironmentException">Thrown when the value is not set.</exception>
    public string Expect(string message)
    {
        if (this.value is null)
        {
            throw new EnvironmentException(message);
        }

        return this.value;
    }

    /// <summary>
    /// Returns the value of the environment variable or throws an exception with the specified error message if the value is not set.
    /// </summary>
    /// <param name="error">A function that returns the exception message.</param>
    /// <returns>The value of the environment variable.</returns>
    /// <exception cref="EnvironmentException">Thrown when the value is not set.</exception>
    public string Expect(Func<string> error)
    {
        if (this.value is null)
        {
            throw new EnvironmentException(error());
        }

        return this.value;
    }

    /// <summary>
    /// Maps the value of the environment variable to a specified type or returns a default value if the value is not set.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="map">A function that maps the value to the specified type.</param>
    /// <param name="defaultValue">The default value to return if the value is not set.</param>
    /// <returns>The mapped value or the default value.</returns>
    public T MapOrDefault<T>(Func<string, T> map, T defaultValue)
    {
        if (this.value is null)
        {
            return defaultValue;
        }

        return map(this.Value);
    }

    /// <summary>
    /// Maps the value of the environment variable to a specified type or returns a default value if the value is not set.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="map">A function that maps the value to the specified type.</param>
    /// <param name="defaultValue">A function that returns the default value if the value is not set.</param>
    /// <returns>The mapped value or the default value.</returns>
    public T MapOrDefault<T>(Func<string, T> map, Func<T> defaultValue)
    {
        if (this.value is null)
        {
            return defaultValue();
        }

        return map(this.Value);
    }

    /// <summary>
    /// Tries to get the value of the environment variable.
    /// </summary>
    /// <param name="value">When this method returns, contains the value of the environment variable if it is set; otherwise, an empty string.</param>
    /// <returns><c>true</c> if the environment variable has a value; otherwise, <c>false</c>.</returns>
    public bool TryGet(out string value)
    {
        value = this.value ?? string.Empty;
        return this.HasValue;
    }

    /// <summary>
    /// Returns the value of the environment variable or a default value if the value is not set.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the value is not set.</param>
    /// <returns>The value of the environment variable or the default value.</returns>
    public string OrDefault(string defaultValue)
    {
        return this.value ?? defaultValue;
    }

    /// <summary>
    /// Returns the value of the environment variable or a default value if the value is not set.
    /// </summary>
    /// <param name="defaultValue">A function that returns the default value if the value is not set.</param>
    /// <returns>The value of the environment variable or the default value.</returns>
    public string OrDefault(Func<string> defaultValue)
    {
        return this.value ?? defaultValue();
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return this.value ?? string.Empty;
    }
}