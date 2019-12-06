using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class Kunde
    {
        [Column("AbtNr")]
        public DateTime Geburtsdatum { get; set; }
        [Key, Column("AbtNr")]

        public int Id { get; set; }
        [Column("AbtNr")]
        public string Nachname { get; set; }
        [Column("AbtNr")]
        public byte[] RowVersion { get; set; }
        [Column("AbtNr")]
        public string Vorname { get; set; }
    }
}