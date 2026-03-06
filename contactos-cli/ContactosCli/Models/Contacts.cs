namespace ContactosCli.Models;

public enum ContactType
{
    Abogado,
    Cliente,
    Demandado,
    Procurador
}

public class Contact
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ContactType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}