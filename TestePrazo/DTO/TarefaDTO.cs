using System.Linq;
using TestePrazo.Domain;

namespace TestePrazo.DTO
{
    public class TarefaDTO
    {
        public IQueryable<Tarefa> Items { get; set; }
    }
}