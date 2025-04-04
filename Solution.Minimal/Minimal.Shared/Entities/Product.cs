using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Shared.Entities;

public class Product
{
    public int Id { get; set; }

    [Display(Name = "Producto")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int Quantity { get; set; }

}
