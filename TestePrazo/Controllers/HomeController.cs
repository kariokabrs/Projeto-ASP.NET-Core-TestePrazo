using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly ITarefaService servico;
        private const string key = "E546C8DF278CD5931069B522E695D4F2";

        public HomeController(ITarefaService servicodi)
        {
            servico = servicodi;
        }

        public ViewResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Tarefa()
        {
            ModelState.Clear();

            TarefaDTO dto = new TarefaDTO();
            dto.Items = await servico.ReadAllAsync();

            //List<ProtectModel> protect = new List<ProtectModel>();

            //foreach (var item in dto.Items)
            //{
            //    var dados = new ProtectModel();
            //    dados.Id = EncryptString(item.Id.ToString(), key);
            //    dados.Nome = item.Nome;
            //    dados.Completa = item.Completa;

            //    protect.Add(dados);
            //}

            return View(dto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddTarefa()
        {
            ModelState.Clear();
            return View();
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(nameof(TarefaViewModel.Id), nameof(TarefaViewModel.Nome), nameof(TarefaViewModel.Completa))] TarefaViewModel editarTarefa)
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
        [Authorize]
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
        [Authorize]
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
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}
