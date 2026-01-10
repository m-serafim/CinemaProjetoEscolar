using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaGestao2223226.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        // Nullable to allow reservations to exist after session is deleted (for refund tracking)
        public int? SessaoId { get; set; }
        public virtual Sessao? Sessao { get; set; }

        [Required]
        [Display(Name = "Utilizador")]
        public string UtilizadorId { get; set; } // stores IdentityUser.Id

        [Display(Name = "Número de Bilhetes")]
        [Range(1, 20)]
        public int NumeroBilhetes { get; set; }

        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva { get; set; }

        [Display(Name = "Lugares Selecionados")]
        [StringLength(500)]
        public string? LugaresSelecionados { get; set; } // Stored as comma-separated string e.g. "A1,A2,A3"

        // Payment Card Details (Simulated - fictitious data for demonstration)
        [Display(Name = "Número do Cartão")]
        [StringLength(19)] // Format: XXXX XXXX XXXX XXXX
        public string? NumeroCartao { get; set; }

        [Display(Name = "Nome no Cartão")]
        [StringLength(100)]
        public string? NomeCartao { get; set; }

        [Display(Name = "Data de Validade")]
        [StringLength(7)] // Format: MM/YYYY
        public string? ValidadeCartao { get; set; }

        [Display(Name = "CVV")]
        [StringLength(4)]
        public string? CVV { get; set; }

        [Display(Name = "Valor Pago")]
        public decimal ValorPago { get; set; }

        // Cancellation and Refund tracking
        [Display(Name = "Cancelada pelo Sistema")]
        public bool CanceladaPeloSistema { get; set; } = false;

        [Display(Name = "Motivo do Cancelamento")]
        [StringLength(500)]
        public string? MotivoCancelamento { get; set; }

        [Display(Name = "Reembolsado")]
        public bool Reembolsado { get; set; } = false;

        [Display(Name = "Data do Reembolso")]
        public DateTime? DataReembolso { get; set; }

        [Display(Name = "Aviso Visualizado")]
        public bool AvisoVisualizado { get; set; } = false;

        // Snapshot of session/movie info for cancelled reservations (preserved when session is deleted)
        [Display(Name = "Nome do Filme")]
        [StringLength(200)]
        public string? FilmeTitulo { get; set; }

        [Display(Name = "Data da Sessão")]
        public DateTime? SessaoDataHora { get; set; }
    }
}