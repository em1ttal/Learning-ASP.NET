using ContactosCli.Models;

namespace ContactosCli.Services;

public interface IContactRepository
{
    List<Contact> LoadAll();
    void SaveAll(List<Contact> contacts);
    void Add(Contact contact);
    List<Contact> Search(string term);
    bool Delete(string id);
    Contact? GetById(string id);
    bool Update(Contact updated);
}