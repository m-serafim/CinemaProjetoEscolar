using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaGestao2223226.Models
{
    public enum StatusFilme
    {
        [Display(Name = "Em Cartaz")]
        EmCartaz = 0,
        [Display(Name = "Brevemente")]
        Brevemente = 1
    }

    public class Filme
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(50)]
        public string Genero { get; set; }

        [Display(Name = "Duração (minutos)")]
        public int DuracaoMinutos { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Display(Name = "URL da Capa")]
        public string CapaUrl { get; set; }

        [Display(Name = "URL do Banner (Hero)")]
        public string? BannerUrl { get; set; }

        [Display(Name = "Elenco")]
        [StringLength(500)]
        public string? Elenco { get; set; }

        [Display(Name = "Realizador")]
        [StringLength(100)]
        public string? Realizador { get; set; }

        [Display(Name = "Classificação Etária")]
        [StringLength(10)]
        public string? ClassificacaoEtaria { get; set; }

        [Display(Name = "URL do Trailer")]
        public string? TrailerUrl { get; set; }

        [Display(Name = "Data de Estreia")]
        public DateTime? DataEstreia { get; set; }

        [Display(Name = "Destaque na Home")]
        public bool DestaqueHome { get; set; } = false;

        [Display(Name = "Status do Filme")]
        public StatusFilme Status { get; set; } = StatusFilme.EmCartaz;
    }
}