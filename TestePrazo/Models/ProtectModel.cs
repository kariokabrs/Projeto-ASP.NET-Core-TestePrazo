using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestePrazo.Models
{
    public class ProtectModel
    {
        public string Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        public bool Completa { get; set; }
    }
}
