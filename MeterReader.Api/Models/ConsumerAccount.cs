using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MeterReader.Api.Models
{
    public class ConsumerAccount
    {
        [Key]
        public int AccountId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
