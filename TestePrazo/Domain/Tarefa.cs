using System.ComponentModel.DataAnnotations;

namespace TestePrazo.Domain
{
    public class Tarefa
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        public bool Completa { get; set; }
    }
}
