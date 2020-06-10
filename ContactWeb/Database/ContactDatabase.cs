using ContactWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactWeb.Database
{
    public interface IContactDatabase
    {
        Contact Insert(Contact contact);
        IEnumerable<Contact> GetContacts();
        Contact GetContact(int id);
        void Delete(int id);
        void Update(int id, Contact updatedContact);
    }

    public class ContactDatabase : IContactDatabase
    {
        private int _counter;
        private readonly List<Contact> _contacts;

        public ContactDatabase()
        {
            if (_contacts == null)
            {
                _contacts = new List<Contact>();
            }
        }

        public Contact GetContact(int id)
        {
            return _contacts.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Contact> GetContacts()
        {
            return _contacts;
        }

        public Contact Insert(Contact contact)
        {
            _counter++;
            contact.Id = _counter;
            _contacts.Add(contact);
            return contact;
        }

        public void Delete(int id)
        {
            var contact = _contacts.SingleOrDefault(x => x.Id == id);
            if (contact != null)
            {
                _contacts.Remove(contact);
            }
        }

        public void Update(int id, Contact updatedContact)
        {
            var contact = _contacts.SingleOrDefault(x => x.Id == id);
            if (contact != null)
            {
                contact.FirstName = updatedContact.FirstName;
                contact.LastName = updatedContact.LastName;
                contact.Email = updatedContact.Email;
                contact.BirthDate = updatedContact.BirthDate;
                contact.Description = updatedContact.Description;
                contact.PhoneNumber = updatedContact.PhoneNumber;
                contact.Addres = updatedContact.Addres;
                contact.Avatar = updatedContact.Avatar;
            }
        }
    }
}
