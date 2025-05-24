using static System.Console;
using System.Diagnostics;
using System.IO;

WriteLine("Hello, World!");

if (args.Length == 0)
{
    WriteLine("proj cli");
    Environment.Exit(0);
}


var cmd = args[0].ToLowerInvariant();
var IsWindows = OperatingSystem.IsWindows();
var dotnetExe = IsWindows ? "dotnet.exe" : "dotnet";

switch(cmd)
{
    case "build":
        
        var ps = Process.Start(dotnetExe, "build");
        ps.WaitForExit();
        Environment.Exit(ps.ExitCode);
        // Add build logic here
        break;

    case "migrations":
        {
            if (args.Length < 2)
            {
                WriteLine("Usage: proj migrations <command> [options]");
                Environment.Exit(1);
            }

            var ec = 0;

            switch (args[1])
            {
                case "add":
                    {
                        if (args.Length < 3)
                        {
                            WriteLine("Usage: proj migrations add <migration_name>");
                            Environment.Exit(1);
                        }

                        var migrationName = args[2];
                        string[] projects = [
                            Path.Join(Directory.GetCurrentDirectory(), "src", "Data.MsSql"),
                            Path.Join(Directory.GetCurrentDirectory(), "src", "Data.PgSql"),
                            Path.Join(Directory.GetCurrentDirectory(), "src", "Data.Sqlite"),
                            Path.Join(Directory.GetCurrentDirectory(), "src", "Data.MySql"),
                        ];


                        foreach (var item in projects)
                        {
                            var si = new ProcessStartInfo
                            {
                                FileName = dotnetExe,
                                Arguments = $"ef migrations add {migrationName} -o ./Migrations",
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WorkingDirectory = item
                            };

                            using (var process = Process.Start(si))
                            {
                                process.WaitForExit();
                                if (process.ExitCode != 0)
                                {
                                    WriteLine($"Error adding migration in {item}: {process.StandardError.ReadToEnd()}");
                                    ec = process.ExitCode;
                                }
                                else
                                {
                                    WriteLine($"Migration {migrationName} added successfully in {item}.");
                                }
                            }

                            si = new ProcessStartInfo
                            {
                                FileName = dotnetExe,
                                Arguments = "format",
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WorkingDirectory = item
                            };

                            using (var process = Process.Start(si))
                            {
                                process.WaitForExit();
                                if (process.ExitCode != 0)
                                {
                                    WriteLine($"Error formatting code in {item}: {process.StandardError.ReadToEnd()}");
                                    ec = process.ExitCode;
                                }
                                else
                                {
                                    WriteLine($"Code formatted successfully in {item}.");
                                }
                            }
                        }
                    }

                    break;
                default:
                    WriteLine($"Unknown migrations command: {args[1]}");
                    Environment.Exit(1);
                break;
            }
        }

        break;
    case "run":
        WriteLine("Running project...");
        // Add run logic here
        break;
    case "test":
        WriteLine("Testing project...");
        // Add test logic here
        break;
    default:
        WriteLine($"Unknown command: {cmd}");
        Environment.Exit(1);
        break;
}
