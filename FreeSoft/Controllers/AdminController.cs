using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FreeSoft.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace FreeSoft.Controllers
{
    public class AdminController : Controller
    {
        private IWebHostEnvironment env;
        public AdminController(IWebHostEnvironment hostingEnvironment)
        {
            env = hostingEnvironment;
        }
        public IActionResult Index()
        {
            using (var context = new SqlConnection(DB.constring))
            {
                ViewBag.Cattegories = new SelectList(context.Query<Cattegory>("select * from Cattegories"), "ID", "Name");
                return View();
            }
        }
        public async Task<IActionResult> AddSoft(string SoftName, string Description, IFormFile Picture, IFormFile SoftFile, string YoutubeLink, string Cattegories)
        {
            var path = env.WebRootPath + $"\\Images\\{Picture.FileName}";
            var file = env.WebRootPath + $"\\UploadFiles\\{SoftFile.FileName}";
            await Task.Run(() =>
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Picture.CopyTo(stream);
                }
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    SoftFile.CopyTo(stream);
                }
                string letters = "QWERTYUIOPASDFGHJKLZXCVBNNM1234567890";
                StringBuilder identity = new StringBuilder();
                Random rand = new Random();
                for (int i = 0; i <= 40; i++)
                {
                    identity.Append(letters[rand.Next(letters.Length - 1)]);
                }
                string Identity = identity.ToString();
                string time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                using (var context = new SqlConnection(DB.constring))
                {
                    context.Query("insert into Soft(Name,Description,FileName,YoutubeLink,SoftIdentity,PictureLink,Cattegory,Date,Rate)" +
                        $"Values('{SoftName}','{Description}','{SoftFile.FileName}','{YoutubeLink}','{Identity}','{Picture.FileName}','{Cattegories}',{time},5)");
                }
            });
            return RedirectToAction("Index", "Home", null);
        }
        public IActionResult EditView(int id)
        {
            using (var context = new SqlConnection(DB.constring))
            {
                var Soft = context.Query<Soft>($"select * from Soft where ID={id}").FirstOrDefault();
                ViewBag.Cattegories = new SelectList(context.Query<Cattegory>("select * from Cattegories"), "ID", "Name");
                return View(Soft);
            }
        }

        public IActionResult Edit(string SoftName, string Description, IFormFile Picture, IFormFile SoftFile, string YoutubeLink, string Cattegories, int FileID)
        {
            using (var context = new SqlConnection(DB.constring))
            {
                string updatecommand = "update Soft ";
                string values = $"set Name='{SoftName}',Description='{Description}',YoutubeLink='{YoutubeLink}',Cattegory='{Cattegories}'";
                if (Picture != null)
                {
                    var path = env.WebRootPath + $"\\Images\\{Picture.FileName}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        Picture.CopyTo(stream);
                    }
                    values += $",PictureLink='{Picture.FileName}'";
                }
                if (SoftFile != null)
                {
                    var file = env.WebRootPath + $"\\UploadFiles\\{SoftFile.FileName}";
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        SoftFile.CopyTo(stream);
                    }
                    values += $",FileName='{SoftFile.FileName}'";
                }
                values += $" where ID={FileID}";
                context.Query(updatecommand + values);
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Delete(int id)
        {
            using (var context=new SqlConnection(DB.constring))
            {
                context.Query($"delete from Soft where ID={id}");
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccountView()
        {
            using (var context=new SqlConnection(DB.constring))
            {
                var list = context.Query<Account>("select * from Acount");
                return View(list);
            }
        }
        public void DeleteAccount(int id)
        {
            using (var context=new SqlConnection(DB.constring))
            {
                context.Query($"delete from Comments where AccountID={id}");
                context.Query($"delete from Acount where ID={id}");
            }
        }
        public void MakeAdmin(int id)
        {
            using (var context=new SqlConnection(DB.constring))
            {
                var account = context.Query<Account>($"update Acount set Role=1 where ID={id}");
            }
        }
    }
}