using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestePrazo.Data;
using TestePrazo.Domain;
using TestePrazo.Models;

namespace TestePrazo.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ApplicationDbContext context;
        public TarefaService(ApplicationDbContext contextdi)
        {
            context = contextdi;
        }

        public async Task<IQueryable<Tarefa>> ReadAllAsync()
        {
            List<Tarefa> items = await context
                .Tarefas
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();

            return items.AsQueryable();
        }

        public async Task<Tarefa> ReadIdAsync(int? id)
        {
            Tarefa item = await context
                .Tarefas
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);
                
            return item;
        }
        public async Task<bool> AddAsync(TarefaViewModel novaTarefa)
        {
            Tarefa entity = new Tarefa
            {
                Nome = novaTarefa.Nome,
                Completa = novaTarefa.Completa,
            };

            await context.Tarefas.AddAsync(entity); 

            int saveResult = await context.SaveChangesAsync(); 

            int myUsuarioId = entity.Id;
            return saveResult == 1;
        }

        public async Task<bool> EditarAsync(TarefaViewModel editarTarefa)
        {
            Tarefa entity = new Tarefa
            {
                Id = editarTarefa.Id,
                Nome = editarTarefa.Nome,
                Completa = editarTarefa.Completa,
            };
            context.Update(entity);
            int saveResult = await context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> DeletarAsync(TarefaViewModel deletarTarefa)
        {
            Tarefa entity = new Tarefa
            {
                Id = deletarTarefa.Id,
                Nome = deletarTarefa.Nome,
                Completa = deletarTarefa.Completa,
            };
            context.Remove(entity);
            int saveResult = await context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
