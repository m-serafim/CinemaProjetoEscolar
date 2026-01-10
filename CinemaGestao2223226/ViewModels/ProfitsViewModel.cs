using System.Collections.Generic;
using CinemaGestao2223226.Models;

namespace CinemaGestao2223226.ViewModels
{
    public class ProfitsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalRefunds { get; set; }
        public decimal NetRevenue => TotalRevenue - TotalRefunds;
        public int TotalTicketsSold { get; set; }
        public int TotalSessions { get; set; }
        public int TotalCustomers { get; set; }
        public List<MovieStats> TopMovies { get; set; } = new();
        public List<TransactionInfo> RecentTransactions { get; set; } = new();
        public List<Reserva> AllReservations { get; set; } = new();
    }

    public class MovieStats
    {
        public string MovieName { get; set; } = string.Empty;
        public int TicketsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TransactionInfo
    {
        public string MovieName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsRefund { get; set; }
    }
}
