using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using FreeSoft.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace FreeSoft.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LoginRegistration()
        {
            var acc = Request.Cookies["Account"];
            if (acc!=null)
            {
                Account account = JsonConvert.DeserializeObject<Account>(Request.Cookies["Account"]);
                return RedirectToAction("AccountView","Account",account);
            }
            return View();
        }
        public JsonResult Login(string Login,string Password)
        {
            using (var context=new SqlConnection(DB.constring))
            {
                var account=context.Query<Account>($"select * from Acount where Login='{Login}'").FirstOrDefault();
                if (account==null)
                {
                    return Json("Неверный Логин");
                }
                else if (account.Password==Password)
                {
                    Response.Cookies.Append("Account", JsonConvert.SerializeObject(account));
                    return Json("Вы успешно зашли в аккаунт");  
                }
                else
                {
                    return Json("Неверный пароль");
                }
            }
        }
        public IActionResult RegisterView()
        {
            return View();
        }
        public JsonResult Register(string Email,string Login,string Password)
        {
            using (var context=new SqlConnection(DB.constring))
            {
                try
                {
                context.Query($"insert into Acount(Login,Password,Email,Role)Values('{Login}','{Password}','{Email}',2)");
                    Account account = new Account { Email=Email,Login=Login,Password=Password, Role=2 };
                    Response.Cookies.Append("Account", JsonConvert.SerializeObject(account));
                }
                catch (Exception ex)
                {
                    return Json("Problem");
                }
                return Json("All Right");
            }
        }
        public IActionResult AccountView(Account account)
        {
            return View(account);
        }
        public IActionResult QuitFromAccount()
        {
            Response.Cookies.Delete("Account");
            return RedirectToAction("Index","Home",null);
        }
    }
}