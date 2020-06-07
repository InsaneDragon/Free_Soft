using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Dapper;
using FreeSoft.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.IO;

namespace FreeSoft.Controllers
{
    public class MoreAboutSoftController : Controller
    {
        private IWebHostEnvironment env;
        public MoreAboutSoftController(IWebHostEnvironment hostingEnvironment)
        {
            env = hostingEnvironment;
        }

        public IActionResult Index(string id)
        {
            using (var context = new SqlConnection(DB.constring))
            {
                Soft soft = context.Query<Soft>($"select * from Soft where SoftIdentity='{id}'").FirstOrDefault();
                var CommentList = context.Query<Comment>($"select * from Comments where SoftID='{id}'");
                ViewBag.Comments = CommentList.ToList();
                return View(soft);
            }
        }
        public void Comment(string Text, string id)
        {
            try
            {
                using (var context = new SqlConnection(DB.constring))
                {
                    string date = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                    context.Query($"insert into Comments(Text,Date,SoftID)values('{Text}','{date}','{id}')");
                }
            }
            catch (Exception ex)
            {
            }
        }
        public ActionResult DownloadFile(string FileName)
        {
            var filepath = $"\\UploadFiles\\{FileName}";
            var path = env.WebRootPath + filepath;
            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }
    }
}