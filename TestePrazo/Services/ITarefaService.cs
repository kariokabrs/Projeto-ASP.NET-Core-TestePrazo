using System.Linq;
using System.Threading.Tasks;
using TestePrazo.Domain;
using System.Collections.Generic;
using TestePrazo.Models;

namespace TestePrazo.Services
{
    public interface ITarefaService
    {
        Task<IQueryable<Tarefa>> ReadAllAsync();
        Task<Tarefa> ReadIdAsync(int? id);
        Task<bool> AddAsync(TarefaViewModel novaTarefa);
        Task<bool> EditarAsync(TarefaViewModel tarefa);
        Task<bool> DeletarAsync(TarefaViewModel deletarTarefa);
    }
}
