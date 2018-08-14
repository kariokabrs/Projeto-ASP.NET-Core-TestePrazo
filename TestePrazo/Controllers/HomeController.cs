using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestePrazo.Models;

namespace TestePrazo.Controllers
{
    public class HomeController : Controller
    {
        ///IDataProtector protector;

        //public HomeController(IDataProtector protectordi)
        //{
        //    protector = protectordi.CreateProtector(GetType().FullName);
        //}
        public ViewResult Index()
        {
            // encriptar querystring
            //var model = _service.GetAll().Select(c => new ContractViewModel
            //{
            //    Id = _protector.Protect(c.Id.ToString()),
            //    Name = c.Name
            //}).ToList();
            //return View(model);

            //@foreach(var entry in Model)
            //{
            //     < div >< a asp - action = "Details" asp - route - id = "@entry.Id" > @entry.Name </ a ></ div >
            // }

            //public IActionResult Details(string id)
            //{
            //    var contract = _service.Find(Convert.ToInt32(_protector.Unprotect(id)));
            //    return View(contract);
            //}

            return View();
        }

        [Authorize]
        public IActionResult Tarefa()
        {
            ViewData["Message"] = "Tarefas";

            return View();
        }

         public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
