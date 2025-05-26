using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Hyprx;

namespace Hyprx.Platform.Process;

public partial class Command
{
    private static readonly ConcurrentDictionary<string, string> ExecutableLocationCache = new();

    /// <summary>
    /// Provides the equivalent functionality of which/where on windows and linux to
    /// determine the full path of an executable found on the PATH.
    /// </summary>
    /// <param name="command">The command to search for.</param>
    /// <param name="prependPaths">Additional paths that will be used to find the executable.</param>
    /// <param name="useCache">Should the lookup use the cached values.</param>
    /// <returns>Null or the full path of the command.</returns>
    /// <exception cref="ArgumentNullException">Throws when command is null.</exception>
    [SuppressMessage("Minor Code Smell", "S6605:Collection-specific \"Exists\" method should be used instead of the \"Any\" extension")]
    public static string? Which(
        string command,
        IEnumerable<string>? prependPaths = null,
        bool useCache = true)
    {
        // https://github.com/actions/runner/blob/592ce1b230985aea359acdf6ed4ee84307bbedc1/src/Runner.Sdk/Util/WhichUtil.cs
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentNullException(nameof(command));

        var rootName = Path.GetFileNameWithoutExtension(command);
        if (useCache && ExecutableLocationCache.TryGetValue(rootName, out var location))
            return location;

#if NETLEGACY
        if (Path.IsPathRooted(command) && File.Exists(command))
        {
            ExecutableLocationCache[command] = command;
            ExecutableLocationCache[rootName] = command;

            return command;
        }
#else
        if (Path.IsPathFullyQualified(command) && File.Exists(command))
        {
            ExecutableLocationCache[command] = command;
            ExecutableLocationCache[rootName] = command;

            return command;
        }
#endif

        var pathSegments = new List<string>();
        if (prependPaths is not null)
            pathSegments.AddRange(prependPaths);

        pathSegments.AddRange(Env.SplitPath());

        for (var i = 0; i < pathSegments.Count; i++)
        {
            pathSegments[i] = Env.ExpandString(pathSegments[i]);
        }

        foreach (var pathSegment in pathSegments)
        {
            if (string.IsNullOrEmpty(pathSegment) || !System.IO.Directory.Exists(pathSegment))
                continue;

            IEnumerable<string> matches = Array.Empty<string>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var pathExt = Env.Get("PATHEXT").OrDefault(string.Empty);
                if (pathExt.IsNullOrWhiteSpace())
                {
                    // XP's system default value for PATHEXT system variable
                    pathExt = ".com;.exe;.bat;.cmd;.vbs;.vbe;.js;.jse;.wsf;.wsh";
                }

                var pathExtSegments = pathExt.ToLowerInvariant()
                                                .Split(
                                                    [";"],
                                                    StringSplitOptions.RemoveEmptyEntries);

                // if command already has an extension.
                if (pathExtSegments.Any(x => command.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        matches = System.IO.Directory.EnumerateFiles(pathSegment, command);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    var result = matches.FirstOrDefault();
                    if (result is null)
                        continue;

                    ExecutableLocationCache[rootName] = result;
                    return result;
                }
                else
                {
                    string searchPattern = $"{command}.*";
                    try
                    {
                        matches = System.IO.Directory.EnumerateFiles(pathSegment, searchPattern);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    var expandedPath = Path.Combine(pathSegment, command);

                    foreach (var match in matches)
                    {
                        foreach (var ext in pathExtSegments)
                        {
                            var fullPath = expandedPath + ext;
                            if (!match.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            ExecutableLocationCache[rootName] = fullPath;
                            return fullPath;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    matches = System.IO.Directory.EnumerateFiles(pathSegment, command);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                var result = matches.FirstOrDefault();
                if (result is null)
                    continue;

                ExecutableLocationCache[rootName] = result;
                return result;
            }
        }

        return null;
    }
}