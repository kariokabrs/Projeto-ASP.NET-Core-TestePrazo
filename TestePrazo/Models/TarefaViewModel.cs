using System.ComponentModel.DataAnnotations;

namespace TestePrazo.Models
{
    public class TarefaViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome {get; set;}
        public bool Completa { get; set; }
    }
}
