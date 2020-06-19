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
    }
}