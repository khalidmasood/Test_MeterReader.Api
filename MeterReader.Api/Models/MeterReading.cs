using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterReader.Api.Models
{
    public class MeterReading
    {

        [Key]
        public int Id { get; set; }

        public int AccountId { get; set; }
        [Column("MeterReadDT")]
        public DateTime MeterReadingDateTime { get; set; }
        [Column("MeterReadVal")]
        public int MeterReadValue { get; set; }
    }
}
