using IDAGroupMVC.Helper;
using IDAGroupMVC.Models;
using IDAGroupMVC.ViewModels.Contacts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static IDAGroupMVC.Services.EmailServices;

namespace IDAGroupMVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _context;
        private readonly IEmailService _emailService;

        public ContactController(DataContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult ContactUs()
        {
            ContactViewModel contactVM = new ContactViewModel
            {
                Setting = _context.Settings.Where(x => x.IsDelete == false).ToList(),
                Contact = new Contact(),
            };
            return View(contactVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUs(Contact contact)
        {
            if (contact == null)
            {
                return RedirectToAction("error", "notfound");
            }

            ContactViewModel contactVM = new ContactViewModel
            {
                Setting = _context.Settings.Where(x => x.IsDelete == false).ToList(),
                Contact = new Contact(),
            };

     
         
            if (contact.Email == null)
                ModelState.AddModelError("", "E-postanızı yazınız!");

            if (!ModelState.IsValid)
                return View(contactVM);

            ///EmailValidator
            bool resultEmail = EmailValidate(contact.Email);
            if (resultEmail == false)
            {
                ModelState.AddModelError("", "E-posta yanlışdır!");
                return View(contactVM);
            }


            if (!ModelState.IsValid)
            {
                return View(contactVM);
            }

            ///CreateContact
            Contact newContact = CreateContact(contact);

            _context.Contacts.Add(newContact);
            _context.SaveChanges();

            ///EmailSend
            var body = EmailBody(contact);
            _emailService.Send(_context.Settings.FirstOrDefault(x=>x.Key == "SenderEmail").Value, "Contact", body);
            TempData["success"] = "Mesajınız gönderildi";
            return RedirectToAction("contactus", "contact");
        }



        private static string EmailBody(Contact contact)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/templates/contactInfo.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{fullname}}", contact.Fullname);
            body = body.Replace("{{subject}}", contact.Subject);
            body = body.Replace("{{email}}", contact.Email);
            body = body.Replace("{{message}}", contact.Text);
            body = body.Replace("{{sendDate}}", contact.CreatedDate.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

            return body;
        }
        private static bool EmailValidate(string emailAddress)
        {
            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            //if email is valid
            if (Regex.IsMatch(emailAddress, pattern))
            {
                return true;
            }
            return false;
        }
       
        private static Contact CreateContact(Contact contact)
        {
            var newContact = new Contact
            {
                Email = contact.Email,
                Fullname = contact.Fullname,
                Text = contact.Text,
                Subject = contact.Subject,
            };
            return newContact;
        }
    }
}
