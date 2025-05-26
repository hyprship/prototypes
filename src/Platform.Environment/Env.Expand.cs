using Hyprx.Platform.Environment;

using static System.Environment;

namespace Hyprx;

#pragma warning disable S127 // update loop counter
public static partial class Env
{
    private enum TokenKind
    {
        None,
        Windows,
        BashVariable,
        BashInterpolation,
    }

    public static EnvResult Expand(ReadOnlySpan<char> template, EnvExpandOptions? options = null)
    {
        var o = options ?? new EnvExpandOptions();
        Func<string, string?> getValue = o.GetVariable ?? GetEnvironmentVariable;
        var setValue = o.SetVariable ?? SetEnvironmentVariable;
        var tokenBuilder = StringBuilderCache.Acquire();
        var output = StringBuilderCache.Acquire();
        var kind = TokenKind.None;
        var remaining = template.Length;
        for (var i = 0; i < template.Length; i++)
        {
            remaining--;
            var c = template[i];
            if (kind == TokenKind.None)
            {
                if (o.WindowsExpansion && c is '%')
                {
                    kind = TokenKind.Windows;
                    continue;
                }

                if (o.UnixExpansion)
                {
                    var z = i + 1;
                    var next = char.MinValue;
                    if (z < template.Length)
                        next = template[z];

                    // escape the $ character.
                    if (c is '\\' && next is '$')
                    {
                        output.Append(next);
                        i++;
                        continue;
                    }

                    if (c is '$')
                    {
                        // can't be a variable if there is no next character.
                        if (next is '{' && remaining > 3)
                        {
                            kind = TokenKind.BashInterpolation;
                            i++;
                            remaining--;
                            continue;
                        }

                        // only a variable if the next character is a letter.
                        if (remaining > 0 && char.IsLetterOrDigit(next))
                        {
                            kind = TokenKind.BashVariable;
                            continue;
                        }
                    }
                }

                output.Append(c);
                continue;
            }

            if (kind == TokenKind.Windows && c is '%')
            {
                if (tokenBuilder.Length == 0)
                {
                    // consecutive %, so just append both characters.
                    output.Append('%', 2);
                    continue;
                }

                var key = tokenBuilder.ToString();
                var value = getValue(key);
                if (value is not null && value.Length > 0)
                    output.Append(value);
                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            if (kind == TokenKind.BashInterpolation && c is '}')
            {
                if (tokenBuilder.Length == 0)
                {
                    // with bash '${}' is a bad substitution.
                    return new EnvResult(
                        new EnvironmentException("${} is a bad substitution. Variable name not provided."));
                }

                var substitution = tokenBuilder.ToString();
                string key = substitution;
                string defaultValue = string.Empty;
                string? message = null;
                if (substitution.Contains(":-"))
                {
                    var parts = substitution.Split([':', '-'], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];
                }
                else if (substitution.Contains(":="))
                {
                    var parts = substitution.Split([':', '='], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];

                    if (o.UnixAssignment)
                    {
                        var v = getValue(key);
                        if (v is null)
                            setValue(key, defaultValue);
                    }
                }
                else if (substitution.Contains(":?"))
                {
                    var parts = substitution.Split([':', '?'], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    if (o.UnixCustomErrorMessage)
                    {
                        message = parts[1];
                    }
                }
                else if (substitution.Contains(":"))
                {
                    var parts = substitution.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];
                }

                if (key.Length == 0)
                {
                    return new EnvResult(new EnvironmentException("Bad substitution, empty variable name."));
                }

                if (!IsValidBashVariable(key.AsSpan()))
                {
                    return new EnvResult(new EnvironmentException($"Bad substitution, invalid variable name {key}."));
                }

                var value = getValue(key);
                if (value is not null)
                    output.Append(value);
                else if (message is not null)
                    return new(new EnvironmentException(message));
                else if (defaultValue.Length > 0)
                    output.Append(defaultValue);
                else
                    return new(new EnvironmentException($"Bad substitution, variable {key} is not set."));

                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            if (kind == TokenKind.BashVariable && (!(char.IsLetterOrDigit(c) || c is '_') || remaining == 0))
            {
                // '\' is used to escape the next character, so don't append it.
                // its used to escape a name like $HOME\\_TEST where _TEST is not
                // part of the variable name.
                bool append = c is not '\\';

                if (remaining == 0 && (char.IsLetterOrDigit(c) || c is '_'))
                {
                    append = false;
                    tokenBuilder.Append(c);
                }

                // rewind one character. Let the previous block handle $ for the next variable
                if (c is '$')
                {
                    append = false;
                    i--;
                }

                var key = tokenBuilder.ToString();
                if (key.Length == 0)
                {
                    return new(new EnvironmentException("Bad substitution, empty variable name."));
                }

                if (o.UnixArgsExpansion && int.TryParse(key, out var index))
                {
                    if (index < 0 || index >= GetCommandLineArgs().Length)
                        return new(new EnvironmentException($"Bad substitution, invalid index {index}."));

                    output.Append(GetCommandLineArgs()[index]);
                    if (append)
                        output.Append(c);

                    tokenBuilder.Clear();
                    kind = TokenKind.None;
                    continue;
                }

                if (!IsValidBashVariable(key.AsSpan()))
                {
                    return new(new EnvironmentException($"Bad substitution, invalid variable name {key}."));
                }

                var value = getValue(key);
                if (value is not null && value.Length > 0)
                    output.Append(value);

                if (value is null)
                    return new(new EnvironmentException($"Bad substitution, variable {key} is not set."));

                if (append)
                    output.Append(c);

                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            tokenBuilder.Append(c);
            if (remaining == 0)
            {
                if (kind is TokenKind.Windows)
                    return new(new EnvironmentException("Bad substitution, missing closing token '%'."));

                if (kind is TokenKind.BashInterpolation)
                    return new(new EnvironmentException("Bad substitution, missing closing token '}'."));
            }
        }

        var result = StringBuilderCache.GetStringAndRelease(output);
        return new EnvResult(string.Empty, result);
    }

    public static string ExpandString(string template, EnvExpandOptions? options = null)
        => ExpandString(template.AsSpan(), options).ToString();

    public static ReadOnlySpan<char> ExpandString(ReadOnlySpan<char> template, EnvExpandOptions? options = null)
    {
        var o = options ?? new EnvExpandOptions();
        Func<string, string?> getValue = o.GetVariable ?? GetEnvironmentVariable;
        var setValue = o.SetVariable ?? SetEnvironmentVariable;
        var tokenBuilder = StringBuilderCache.Acquire();
        var output = StringBuilderCache.Acquire();
        var kind = TokenKind.None;
        var remaining = template.Length;
        for (var i = 0; i < template.Length; i++)
        {
            remaining--;
            var c = template[i];
            if (kind == TokenKind.None)
            {
                if (o.WindowsExpansion && c is '%')
                {
                    kind = TokenKind.Windows;
                    continue;
                }

                if (o.UnixExpansion)
                {
                    var z = i + 1;
                    var next = char.MinValue;
                    if (z < template.Length)
                        next = template[z];

                    // escape the $ character.
                    if (c is '\\' && next is '$')
                    {
                        output.Append(next);
                        i++;
                        continue;
                    }

                    if (c is '$')
                    {
                        // can't be a variable if there is no next character.
                        if (next is '{' && remaining > 3)
                        {
                            kind = TokenKind.BashInterpolation;
                            i++;
                            remaining--;
                            continue;
                        }

                        // only a variable if the next character is a letter.
                        if (remaining > 0 && char.IsLetterOrDigit(next))
                        {
                            kind = TokenKind.BashVariable;
                            continue;
                        }
                    }
                }

                output.Append(c);
                continue;
            }

            if (kind == TokenKind.Windows && c is '%')
            {
                if (tokenBuilder.Length == 0)
                {
                    // consecutive %, so just append both characters.
                    output.Append('%', 2);
                    continue;
                }

                var key = tokenBuilder.ToString();
                var value = getValue(key);
                if (value is not null && value.Length > 0)
                    output.Append(value);
                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            if (kind == TokenKind.BashInterpolation && c is '}')
            {
                if (tokenBuilder.Length == 0)
                {
                    // with bash '${}' is a bad substitution.
                    throw new EnvironmentException("${} is a bad substitution. Variable name not provided.");
                }

                var substitution = tokenBuilder.ToString();
                string key = substitution;
                string defaultValue = string.Empty;
                string? message = null;
                if (substitution.Contains(":-"))
                {
                    var parts = substitution.Split([':', '-'], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];
                }
                else if (substitution.Contains(":="))
                {
                    var parts = substitution.Split([':', '='], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];

                    if (o.UnixAssignment)
                    {
                        var v = getValue(key);
                        if (v is null)
                            setValue(key, defaultValue);
                    }
                }
                else if (substitution.Contains(":?"))
                {
                    var parts = substitution.Split([':', '?'], StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    if (o.UnixCustomErrorMessage)
                    {
                        message = parts[1];
                    }
                }
                else if (substitution.Contains(":"))
                {
                    var parts = substitution.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0];
                    defaultValue = parts[1];
                }

                if (key.Length == 0)
                {
                    throw new EnvironmentException("Bad substitution, empty variable name.");
                }

                if (!IsValidBashVariable(key.AsSpan()))
                {
                    throw new EnvironmentException($"Bad substitution, invalid variable name {key}.");
                }

                var value = getValue(key);
                if (value is not null)
                    output.Append(value);
                else if (message is not null)
                    throw new EnvironmentException(message);
                else if (defaultValue.Length > 0)
                    output.Append(defaultValue);
                else
                    throw new EnvironmentException($"Bad substitution, variable {key} is not set.");

                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            if (kind == TokenKind.BashVariable && (!(char.IsLetterOrDigit(c) || c is '_') || remaining == 0))
            {
                // '\' is used to escape the next character, so don't append it.
                // its used to escape a name like $HOME\\_TEST where _TEST is not
                // part of the variable name.
                bool append = c is not '\\';

                if (remaining == 0 && (char.IsLetterOrDigit(c) || c is '_'))
                {
                    append = false;
                    tokenBuilder.Append(c);
                }

                // rewind one character. Let the previous block handle $ for the next variable
                if (c is '$')
                {
                    append = false;
                    i--;
                }

                var key = tokenBuilder.ToString();
                if (key.Length == 0)
                {
                    throw new EnvironmentException("Bad substitution, empty variable name.");
                }

                if (o.UnixArgsExpansion && int.TryParse(key, out var index))
                {
                    if (index < 0 || index >= GetCommandLineArgs().Length)
                        throw new EnvironmentException($"Bad substitution, invalid index {index}.");

                    output.Append(GetCommandLineArgs()[index]);
                    if (append)
                        output.Append(c);

                    tokenBuilder.Clear();
                    kind = TokenKind.None;
                    continue;
                }

                if (!IsValidBashVariable(key.AsSpan()))
                {
                    throw new EnvironmentException($"Bad substitution, invalid variable name {key}.");
                }

                var value = getValue(key);
                if (value is not null && value.Length > 0)
                    output.Append(value);

                if (value is null)
                    throw new EnvironmentException($"Bad substitution, variable {key} is not set.");

                if (append)
                    output.Append(c);

                tokenBuilder.Clear();
                kind = TokenKind.None;
                continue;
            }

            tokenBuilder.Append(c);
            if (remaining == 0)
            {
                if (kind is TokenKind.Windows)
                    throw new EnvironmentException("Bad substitution, missing closing token '%'.");

                if (kind is TokenKind.BashInterpolation)
                    throw new EnvironmentException("Bad substitution, missing closing token '}'.");
            }
        }

        var set = new char[output.Length];
        output.CopyTo(0, set, 0, output.Length);
        output.Clear();
        return set;
    }

    private static bool IsValidBashVariable(ReadOnlySpan<char> input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            if (i == 0 && !char.IsLetter(input[i]))
                return false;

            if (!char.IsLetterOrDigit(input[i]) && input[i] is not '_')
                return false;
        }

        return true;
    }
}