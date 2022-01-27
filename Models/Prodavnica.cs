using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace Models
{
    [Table("Prodavnica")]
    public class Prodavnica
    {
        [Key]
        public int IDProdavnica { get; set; }

        [Required]
        [MaxLength(50)]
        public string Naziv { get; set; }

        [Required]
        public List<Proizvod> Meni { get; set; }

        [Required]
        public List<Sastojak> Frizider { get; set; }
    }
}
