using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaGestao.Models
{
    public class Filme
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [StringLength(50)]
        public string Genero { get; set; }

        [Display(Name = "Duração (minutos)")]
        public int DuracaoMinutos { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "URL da Capa")]
        public string CapaUrl { get; set; }

        public virtual ICollection<Sessao> Sessoes { get; set; }
    }
}