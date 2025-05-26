using Hyprx.Text.DotEnv.Documents;
using Hyprx.Text.DotEnv.Serialization;

namespace Hyprx.Text.DotEnv;

public static class DotEnvFile
{
    public static DotEnvDocument Parse(string value, DotEnvSerializerOptions? options = null)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return Serializer.DeserializeDocument(value, options);
    }

    public static DotEnvDocument Parse(DotEnvLoadOptions options)
    {
        if (options.Files.Count == 1 && options.Content is null)
            return Serializer.DeserializeDocument(options.Files[0], options);
        else if (options.Files.Count == 0 && options.Content is not null)
            return Serializer.DeserializeDocument(options.Content, options);
        else if (options.Files.Count == 0 && options.Content is null)
            return [];

        DotEnvDocument doc = new();
        if (options.Files.Count > 0)
        {
            foreach (var file in options.Files)
            {
                var clone = (DotEnvLoadOptions)options.Clone();
                clone.ExpandVariables = doc;
                var fileContent = File.ReadAllText(file);
                var d = (IDictionary<string, string>)DotEnvSerializer.DeserializeDocument(fileContent, options);
                foreach (var pair in d)
                {
                    doc[pair.Key] = pair.Value;
                }
            }
        }

        if (options.Content is not null)
        {
            var clone = (DotEnvLoadOptions)options.Clone();
            clone.ExpandVariables = doc;
            var d = (IDictionary<string, string>)DotEnvSerializer.DeserializeDocument(options.Content, options);
            foreach (var pair in d)
            {
                doc[pair.Key] = pair.Value;
            }
        }

        return doc;
    }

    public static string Stringify(DotEnvDocument document, DotEnvSerializerOptions? options = null)
        => DotEnvSerializer.SerializeDocument(document,  options);

    public static string Stringify(IEnumerable<KeyValuePair<string, string>> dictionary, DotEnvSerializerOptions? options = null)
        => DotEnvSerializer.SerializeDictionary(dictionary, options);

    public static void Load(DotEnvLoadOptions options)
    {
        var doc = Parse(options);
        foreach (var entry in doc)
        {
            if (entry is DotEnvEntry var && (options.OverrideEnvironment || !Env.Has(var.Name)))
            {
                Env.Set(var.Name, var.Value);
            }
        }
    }
}