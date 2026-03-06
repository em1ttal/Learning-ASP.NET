using System.Text.Json;
using ContactosCli.Models;

namespace ContactosCli.Services;

public class ContactRepository : IContactRepository
{
    private readonly string _filePath;

    // JSON config: pretty-print and write enum names instead of numbers
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public ContactRepository(string filePath = "contacts.json")
    {
        _filePath = filePath;
    }

    public List<Contact> LoadAll()
    {
        if (!File.Exists(_filePath))
            return [];

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Contact>>(json, _jsonOptions) ?? [];
    }

    public void SaveAll(List<Contact> contacts)
    {
        string json = JsonSerializer.Serialize(contacts, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public void Add(Contact contact)
    {
        var contacts = LoadAll();
        contact.Id = Guid.NewGuid().ToString("N")[..8]; // short unique ID like "a3f7b2c1"
        contact.CreatedAt = DateTime.Now;
        contacts.Add(contact);
        SaveAll(contacts);
    }

    public List<Contact> Search(string term)
    {
        var contacts = LoadAll();
        string lowerTerm = term.ToLower();

        return contacts.Where(c =>
            c.Name.ToLower().Contains(lowerTerm) ||
            c.Email.ToLower().Contains(lowerTerm) ||
            c.PhoneNumber.Contains(term)
        ).ToList();
    }

    public bool Delete(string id)
    {
        var contacts = LoadAll();
        int removed = contacts.RemoveAll(c => c.Id == id);
        if (removed > 0)
        {
            SaveAll(contacts);
            return true;
        }
        return false;
    }

    public Contact? GetById(string id)
    {
        var contacts = LoadAll();
        return contacts.FirstOrDefault(c => c.Id == id);
    }

    public bool Update(Contact updated)
    {
        var contacts = LoadAll();
        int index = contacts.FindIndex(c => c.Id == updated.Id);
        if (index == -1)
            return false;

        contacts[index] = updated;
        SaveAll(contacts);
        return true;
    }
}