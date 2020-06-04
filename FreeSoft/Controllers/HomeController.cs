using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FreeSoft.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Auction_5._0;

namespace FreeSoft.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int ?message)
            {
            using (var context = new SqlConnection(DB.constring))
            {
                IEnumerable<Soft> list = new List<Soft>();
                if (message==null)
                {
                 list = context.Query<Soft>("select * from Soft").OrderByDescending(p=>p.ID);
                }
                else
                {
                    list = context.Query<Soft>($"select * from Soft where Cattegory={message}").Take(20);
                }
                List<Cattegory> Cats = new List<Cattegory>();
                foreach (var item in list)
                {
                Cats.Add(context.Query<Cattegory>($"select * from Cattegories where ID={item.Cattegory}").FirstOrDefault());
                }
                ViewBag.Cats = Cats.ToList();
                ViewBag.Role = 2;
                if (Request.Cookies["Account"]!=null)
                {
                    ViewBag.Role = JsonConvert.DeserializeObject<Account>(Request.Cookies["Account"]).Role;
                }
                return View(list.ToList());
            }
        }
        public IActionResult SearchByName(string Search)
        {
            using (var context = new SqlConnection(DB.constring))
            {
                var list = context.Query<Soft>("select * from Soft").Where(p => Leveinshtein.GetLevenshteinDistance(p.Name.ToLower(), Search.ToLower()) <= 3).Take(20);
                list.OrderBy(p => Leveinshtein.GetLevenshteinDistance(p.Name, Search));
                List<Cattegory> Cats = new List<Cattegory>();
                foreach (var item in list)
                {
                    Cats.Add(context.Query<Cattegory>($"select * from Cattegories where ID={item.Cattegory}").FirstOrDefault());
                }
                ViewBag.Cats = Cats.ToList();
                return View(list.ToList());
            }
        }
    }
}
