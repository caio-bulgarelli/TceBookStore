using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    /// <summary>
    /// ISBN, Autor, Nome, Preço, Data Publicação, Imagem da Capa
    /// </summary>
    public class Book : Model
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [RegularExpression("([0-9]{13}|[0-9]{10})", ErrorMessage = "O campo ISBN deve conter 10 ou 13 caracteres.")]
        [Display(Name = "ISBN ou e-ISBN")]
        public string Isbn { get; set; }

        [Required(ErrorMessage = "O campo Autor é obrigatório.")]
        [StringLength(128, MinimumLength = 3)]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        [StringLength(128, MinimumLength = 3)]
        [Display(Name = "Título", Description = "Título com subtítulo.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "O campo Preço é obrigatório.")]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Preço")]
        public double Price { get; set; }

        [Required(ErrorMessage = "O campo Ano da Publicação é obrigatório.")]
        [Range(1700, 2019)]
        [Display(Name = "Ano da Publicação")]
        public int PublicationDate { get; set; }

        [Display(Name = "Capa")]
        public string Cover { get; set; }
    }
}
