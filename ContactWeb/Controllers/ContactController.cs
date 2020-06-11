using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContactWeb.Database;
using ContactWeb.Domain;
using ContactWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactDatabase _contactDatabase;
        private readonly IWebHostEnvironment _hostEnvironment;

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
                PhotoUrl = contactFromDb.PhotoUrl
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

            //if (contact.Avatar != null)
            //{
            //    file = GetBytesFromFile(contact.Avatar);
            //}
            //else
            //{
            //    file = new byte[] { };
            //}

            

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
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(contact.Avatar.FileName);
                var path = Path.Combine(_hostEnvironment.WebRootPath, "photos", uniqueFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    contact.Avatar.CopyTo(stream);
                }

                contactToDb.PhotoUrl = "/photos/" + uniqueFileName;
            }

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
                PhotoUrl = contactFromDb.PhotoUrl
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

            //if (contact.Avatar != null)
            //{
            //    var bytes = GetBytesFromFile(contact.Avatar);
            //    contactToDb.Avatar = bytes;
            //}

            if (contact.Avatar != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(contact.Avatar.FileName);
                var path = Path.Combine(_hostEnvironment.WebRootPath, "photos", uniqueFileName);
                Contact contactFromDb = _contactDatabase.GetContact(id);

                if (!string.IsNullOrEmpty(contactFromDb.PhotoUrl))
                {
                    var prevPath = Path.Combine(_hostEnvironment.WebRootPath, "photos", contactFromDb.PhotoUrl.Substring(8));
                    System.IO.File.Delete(prevPath);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    contact.Avatar.CopyTo(stream);
                }

                contactToDb.PhotoUrl = "/photos/" + uniqueFileName;
            }
            else
            {
                Contact contactFromDb = _contactDatabase.GetContact(id);

                if (!string.IsNullOrEmpty(contactFromDb.PhotoUrl))
                    contactToDb.PhotoUrl = contactFromDb.PhotoUrl;
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
            Contact contact = _contactDatabase.GetContact(id);
            if (!string.IsNullOrEmpty(contact.PhotoUrl))
            {
                var prevPath = Path.Combine(_hostEnvironment.WebRootPath, "photos", contact.PhotoUrl.Substring(8));
                System.IO.File.Delete(prevPath);
            }

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

        //private string UploadPhoto(IFormFile photo)
        //{
        //    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
        //    string pathName = Path.Combine(_hostEnvironment.WebRootPath, "photos");
        //    string fileNameWithPath = Path.Combine(pathName, uniqueFileName);

        //    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
        //    {
        //        photo.CopyTo(stream);
        //    }
        //    return uniqueFileName;
        //}
    }
}
