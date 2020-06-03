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
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
                foreach (var item in CommentList.ToList())
                {
                    item.Login = context.Query<string>($"select Login from Acount where ID={item.AccountID}").FirstOrDefault();
                }
                ViewBag.Comments = CommentList.ToList();
                return View(soft);
            }
        }
        public JsonResult Comment(string Text, string id)
        {
            try
            {
                using (var context = new SqlConnection(DB.constring))
                {
                    var account = Request.Cookies["Account"];
                    if (account == null && string.IsNullOrEmpty(Text))
                    {
                        return Json("Error");
                    }
                    var acc = JsonConvert.DeserializeObject<Account>(account);
                    string date = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                    context.Query($"insert into Comments(Text,Date,SoftID,AccountID)values('{Text}','{date}','{id}','{acc.ID}')");
                    return Json(new Account { Login = acc.Login });
                }
            }
            catch (Exception ex)
            {
                return Json("Error");
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