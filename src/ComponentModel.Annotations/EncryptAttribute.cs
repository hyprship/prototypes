namespace Hyprx.ComponentModel.Annotations;

[AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = false)]
public class EncryptAttribute : System.Attribute
{
    public EncryptAttribute(string providerName = "default")
    {
        this.ProviderName = providerName;
    }

    public string ProviderName { get; set; }
}