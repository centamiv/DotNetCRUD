using System.Linq;
using DotNetCrud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models2;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private AdventureWorksLT2016Context DatabaseContext { get; }

        public HomeController(AdventureWorksLT2016Context db)
        {
            DatabaseContext = db;
        }

        public IActionResult Index()
        {
            new DotNetCrud<SalesOrderDetail>(HttpContext, DatabaseContext)
                .TableSearch((query, value) => query)
                .TableFields("OrderQty", "UnitPrice", "LineTotal", "SalesOrder.ShipMethod")
                .Build();

            return View();
        }
    }
}
