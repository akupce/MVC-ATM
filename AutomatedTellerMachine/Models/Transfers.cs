using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutomatedTellerMachine.Models
{
    public class Transfers
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Receiver")]
        public int ReceiverId { get; set; }
        public virtual CheckingAccount Receiver { get; set; }
        public int SenderId { get; set; }
        public virtual CheckingAccount Sender { get; set; }
    }
}
