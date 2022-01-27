using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace Models
{
    [Table("Sastojak")]
    public class Sastojak
    {
        [Key]
        public int IDSastojak { get; set; }

        [Required]
        [MaxLength(50)]
        public string Naziv { get; set; }

        [Required]
        public int Kolicina { get; set; }
    }
}