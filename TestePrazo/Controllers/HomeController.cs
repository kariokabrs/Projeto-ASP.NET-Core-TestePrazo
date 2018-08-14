using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestePrazo.Domain;
using TestePrazo.DTO;
using TestePrazo.Models;
using TestePrazo.Services;

namespace TestePrazo.Controllers
{
    public class HomeController : Controller
    {
        ///IDataProtector protector;

        //public HomeController(IDataProtector protectordi)
        //{
        //    protector = protectordi.CreateProtector(GetType().FullName);
        //}

        private readonly ITarefaService servico;

        public HomeController(ITarefaService servicodi) => servico = servicodi;


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
        [HttpGet]
        public async Task<IActionResult> Tarefa()
        {

            ModelState.Clear();

            TarefaDTO dto = new TarefaDTO();
            dto.Items = await servico.ReadAllAsync();

            return View(dto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddTarefa()
        {
            ModelState.Clear();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTarefa([Bind(nameof(TarefaViewModel.Nome), nameof(TarefaViewModel.Completa))] TarefaViewModel novaTarefa)
        {
            if (await TryUpdateModelAsync<TarefaViewModel>(novaTarefa, "tarefa", s => s.Nome, s => s.Completa))
            {
                await servico.AddAsync(novaTarefa);
                return RedirectToAction(nameof(Tarefa));
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tarefa retorno = await servico.ReadIdAsync(id);
            if (retorno == null)
            {
                return NotFound();
            }
            return View(retorno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(nameof(TarefaViewModel.Id),nameof(TarefaViewModel.Nome), nameof(TarefaViewModel.Completa))] TarefaViewModel editarTarefa)
        {
            if (id != editarTarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await servico.EditarAsync(editarTarefa);
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;
                }
                return RedirectToAction(nameof(Tarefa));
            }
            return View(editarTarefa);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tarefa retorno = await servico.ReadIdAsync(id);
            if (retorno == null)
            {
                return NotFound();
            }
           
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Não foi possível deletar. Contate o administrador.";
            }

            return View(retorno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool? saveChangesError = false)
        {
            Tarefa retorno = await servico.ReadIdAsync(id);

            if (retorno == null)
            {
                return RedirectToAction(nameof(Tarefa));
            }

            try
            {
                TarefaViewModel model = new TarefaViewModel()
                {
                    Id = retorno.Id,
                    Nome = retorno.Nome,
                    Completa = retorno.Completa
                };

                await servico.DeletarAsync(model);
                return RedirectToAction(nameof(Tarefa));
            }
            catch (DbUpdateException )
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
