using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContactWeb.Database;
using ContactWeb.Domain;
using ContactWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactDatabase _contactDatabase;

        public ContactController(IContactDatabase contacts)
        {
            _contactDatabase = contacts;
        }

        public IActionResult Index()
        {
            var contacts = _contactDatabase.GetContacts()
                .Select(item => new ContactIndexModel()
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                });

            return View(contacts);
        }

        public IActionResult Detail(int id)
        {
            var contactFromDb = _contactDatabase.GetContact(id);

            var contact = new ContactDetailModel()
            {
                FirstName = contactFromDb.FirstName,
                LastName = contactFromDb.LastName,
                PhoneNumber = contactFromDb.PhoneNumber,
                Address = contactFromDb.Addres,
                Email = contactFromDb.Email,
                Description = contactFromDb.Description,
                BirthDate = contactFromDb.BirthDate,
                Avatar = contactFromDb.Avatar
            };

            return View(contact);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ContactCreateModel contact)
        {
            if (!TryValidateModel(contact))
            {
                return View(contact);
            }

            byte[] file;

            if (contact.Avatar != null)
            {
                file = GetBytesFromFile(contact.Avatar);
            }
            else
            {
                file = new byte[] { };
            }

            var contactToDb = new Contact()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                PhoneNumber = contact.PhoneNumber,
                Addres = contact.Address,
                Email = contact.Email,
                Description = contact.Description,
                BirthDate = contact.BirthDate,
                Avatar = file
            };

            _contactDatabase.Insert(contactToDb);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var contactFromDb = _contactDatabase.GetContact(id);

            var contact = new ContactEditModel()
            {
                FirstName = contactFromDb.FirstName,
                LastName = contactFromDb.LastName,
                PhoneNumber = contactFromDb.PhoneNumber,
                Address = contactFromDb.Addres,
                Email = contactFromDb.Email,
                Description = contactFromDb.Description,
                BirthDate = contactFromDb.BirthDate,
                FileBytes = contactFromDb.Avatar
            };

            return View(contact);
        }

        [HttpPost]
        public IActionResult Edit(int id, ContactEditModel contact)
        {
            if (!TryValidateModel(contact))
            {
                return View(contact);
            }

            var contactToDb = new Contact()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                PhoneNumber = contact.PhoneNumber,
                Addres = contact.Address,
                Email = contact.Email,
                Description = contact.Description,
                BirthDate = contact.BirthDate
            };

            if (contact.Avatar != null)
            {
                var bytes = GetBytesFromFile(contact.Avatar);
                contactToDb.Avatar = bytes;
            }

            _contactDatabase.Update(id, contactToDb);

            return RedirectToAction("Detail", new { Id = id });
        }

        public IActionResult Delete(int id)
        {
            var contactFromDb = _contactDatabase.GetContact(id);

            var contact = new ContactDeleteModel()
            {
                Id = contactFromDb.Id,
                FirstName = contactFromDb.FirstName,
                LastName = contactFromDb.LastName,
            };

            return View(contact);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            _contactDatabase.Delete(id);

            return RedirectToAction("Index");
        }

        public Byte[] GetBytesFromFile(IFormFile file)
        {
            var extension = new FileInfo(file.FileName).Extension;
            if (extension == ".jpg" || extension == ".png" || extension == ".PNG")
            {
                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
            else
            {
                return new byte[]{ };
            }
        }
    }
}
