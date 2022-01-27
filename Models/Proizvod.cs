using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace Models
{
    [Table("Proizvod")]
    public class Proizvod
    {
        [Key]
        public int IdProizvod { get; set; }

        [Required]
        [MaxLength(30)]
        public string Naziv { get; set; }

        [Required]
        public List<Sastojak> Sastojci { get; set; }
    }
}