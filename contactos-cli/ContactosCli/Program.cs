using ContactosCli.Models;
using ContactosCli.Services;

var repo = new ContactRepository();

if (args.Length == 0)
{
    ShowHelp();
    return;
}

switch (args[0].ToLower())
{
    case "add":
        HandleAdd(args);
        break;
    case "list":
        HandleList();
        break;
    case "search":
        HandleSearch(args);
        break;
    case "delete":
        HandleDelete(args);
        break;
    case "edit":
        HandleEdit(args);
        break;
    default:
        Console.WriteLine($"Unknown command: {args[0]}");
        ShowHelp();
        break;
}

void HandleAdd(string[] args)
{
    if (args.Length < 3)
    {
        Console.WriteLine("Usage: add <Name> <Type> [PhoneNumber] [Email]");
        Console.WriteLine("Types: Abogado, Cliente, Demandado, Procurador");
        return;
    }

    if (!Enum.TryParse<ContactType>(args[2], ignoreCase: true, out var Type))
    {
        Console.WriteLine($"Type inválido: {args[2]}");
        Console.WriteLine("Types válidos: Abogado, Cliente, Demandado, Procurador");
        return;
    }

    var contact = new Contact
    {
        Name = args[1],
        Type = Type,
        PhoneNumber = args.Length > 3 ? args[3] : string.Empty,
        Email = args.Length > 4 ? args[4] : string.Empty
    };

    repo.Add(contact);
    Console.WriteLine($"Contact added: {contact.Name} (ID: {contact.Id})");
}

void HandleList()
{
    var contacts = repo.LoadAll();

    if (contacts.Count == 0)
    {
        Console.WriteLine("No contacts found.");
        return;
    }

    Console.WriteLine($"{"ID",-10} {"Name",-25} {"Type",-15} {"Teléfono",-15} {"Email",-25}");
    Console.WriteLine(new string('-', 90));

    foreach (var c in contacts)
    {
        Console.WriteLine($"{c.Id,-10} {c.Name,-25} {c.Type,-15} {c.PhoneNumber,-15} {c.Email,-25}");
    }

    Console.WriteLine($"\nTotal: {contacts.Count} contacts");
}

void HandleSearch(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: search <term>");
        return;
    }

    var results = repo.Search(args[1]);

    if (results.Count == 0)
    {
        Console.WriteLine($"No contacts found matching '{args[1]}'");
        return;
    }

    Console.WriteLine($"Found {results.Count} result(s):\n");
    foreach (var c in results)
    {
        Console.WriteLine($"  [{c.Id}] {c.Name} ({c.Type}) - {c.PhoneNumber} - {c.Email}");
    }
}

void HandleDelete(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: delete <id>");
        return;
    }

    if (repo.Delete(args[1]))
        Console.WriteLine($"Contact {args[1]} deleted.");
    else
        Console.WriteLine($"Contact {args[1]} not found.");
}

void HandleEdit(string[] args)
{
    if (args.Length < 4)
    {
        Console.WriteLine("Usage: edit <id> <field> <value>");
        Console.WriteLine("Fields: Name, PhoneNumber, Email, Type");
        return;
    }

    string id = args[1];
    string field = args[2].ToLower();
    string value = args[3];

    var contact = repo.GetById(id);
    if (contact == null)
    {
        Console.WriteLine($"Contact {id} not found.");
        return;
    }

    switch (field)
    {
        case "Name":
            contact.Name = value;
            break;
        case "PhoneNumber":
            contact.PhoneNumber = value;
            break;
        case "Email":
            contact.Email = value;
            break;
        case "Type":
            if (!Enum.TryParse<ContactType>(value, ignoreCase: true, out var Type))
            {
                Console.WriteLine($"Type inválido: {value}");
                return;
            }
            contact.Type = Type;
            break;
        default:
            Console.WriteLine($"Unknown field: {field}");
            return;
    }

    repo.Update(contact);
    Console.WriteLine($"Contact {id} updated: {field} = {value}");
}

void ShowHelp()
{
    Console.WriteLine("""
    Contactos CLI - Contact Manager

    Commands:
      add <Name> <Type> [PhoneNumber] [Email]   Add a new contact
      list                                      List all contacts
      search <term>                             Search by name, Email, or phone
      edit <id> <field> <value>                 Edit a contact field
      delete <id>                               Delete a contact

    Types: Abogado, Cliente, Demandado, Procurador

    Examples:
      dotnet run -- add "Juan García" Cliente 612345678 juan@mail.com
      dotnet run -- list
      dotnet run -- search García
      dotnet run -- edit a3f7b2c1 Email nuevo@mail.com
      dotnet run -- delete a3f7b2c1
    """);
}