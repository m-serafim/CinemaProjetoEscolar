using CinemaGestao.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaGestao.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int SessaoId { get; set; }
        public virtual Sessao Sessao { get; set; }

        [Required]
        [Display(Name = "Utilizador")]
        public string UtilizadorId { get; set; } // stores IdentityUser.Id

        [Display(Name = "Número de Bilhetes")]
        [Range(1, 20)]
        public int NumeroBilhetes { get; set; }

        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva { get; set; }
    }
}