using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Models;
using System.Collections.Generic;

namespace Jun_2021.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ProdavnicaController : ControllerBase
    {
        public Context Context { get; set; }

        public ProdavnicaController(Context context)
        {
            Context = context;
        }

        // GET 

        [HttpGet]
        [Route("Vrati_sve_prodavnice")]
        public ActionResult VratiProdavnice()
        {
            var prodavnice = Context.Prodavnice
                                  .Include(p => p.Meni)
                                  .ThenInclude(p => p.Sastojci)
                                  .Include(p => p.Frizider)
                                  .ToList();

            return Ok(prodavnice);
        }

        [HttpGet]
        [Route("Vrati_prodavnicu/{id}")]
        public ActionResult VratiProdavnicu(int id)
        {
            if (id < 0)
                return BadRequest("Prodavnica sa ovim Id-jem ne postoji!");

            try
            {
                var shop = Context.Prodavnice
                                           .Include(p => p.Meni)
                                           .ThenInclude(p => p.Sastojci)
                                           .Include(p => p.Frizider)
                                           .Where(p => p.IDProdavnica == id).FirstOrDefault();

                return Ok(shop);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT

        [HttpPut]
        [Route("Naruci_proizvod/{IdProdavnice}/{Proizvod}/{Kolicina}")]

        public async Task<ActionResult> Naruci(int IdProdavnice, string Proizvod, int brNarudzbina)
        {
            if (IdProdavnice < 0) return BadRequest("Prodavnica sa ovim Id-jem ne postoji!");
            if (Proizvod == "") return BadRequest("Morate uneti ime proizvoda!");
            if (brNarudzbina <= 0) return BadRequest("Unesite ispravno kolicinu!");

            try
            {
                var Shop = Context.Prodavnice
                                .Include(p => p.Meni)
                                .ThenInclude(p => p.Sastojci)
                                .Include(p => p.Frizider)
                                .Where(p => p.IDProdavnica == IdProdavnice)
                                .FirstOrDefault();

                if (Shop != null)
                {
                    var Nedostaje = new List<Sastojak>();

                    Shop.Meni.ForEach(p =>
                    {
                        if (p.Naziv == Proizvod)
                        {
                            p.Sastojci.ForEach(s =>
                            {

                                var nadjen = 0;
                                Shop.Frizider.ForEach(f =>
                                {
                                    if (f.Naziv == s.Naziv)
                                    {
                                        nadjen = 1;

                                        if (f.Kolicina > brNarudzbina * s.Kolicina)
                                        {
                                            f.Kolicina = f.Kolicina - brNarudzbina * s.Kolicina;
                                        }
                                        else
                                        {
                                            var Sastojak = new Sastojak();
                                            Sastojak.Naziv = s.Naziv;
                                            Sastojak.Kolicina = brNarudzbina * s.Kolicina - f.Kolicina;

                                            Nedostaje.Add(Sastojak);
                                        }
                                    }
                                });
                                if (nadjen == 0)
                                    Nedostaje.Add(s);
                            });
                        }
                    });

                    if (Nedostaje == null)
                    {
                        Context.Prodavnice.Update(Shop);
                        await Context.SaveChangesAsync();
                        return Ok(Nedostaje);
                    }
                    else
                        return Ok(Nedostaje);
                }
                else
                    return BadRequest("Prodavnica sa ovim Id-em ne postoji!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("DodajSastojkeuFrizider/{idProdavnice}/{Sastojak}/{Kolicina}")]
        public async Task<ActionResult> DodajSastojak(int id, string Sastojak, int Kolicina)
        {
            try
            {
                var Shop = Context.Prodavnice.Include(p => p.Frizider).Where(p => p.IDProdavnica == id).FirstOrDefault();

                if (Shop != null)
                {
                    foreach (var S in Shop.Frizider)
                    {
                        if (S.Naziv == Sastojak)
                        {
                            S.Kolicina = S.Kolicina + Kolicina * 5;

                            Context.Prodavnice.Update(Shop);
                            await Context.SaveChangesAsync();
                            return Ok("Dotate su nove kolicine proizvoda!");
                        }
                    }

                    return BadRequest("Ovaj proizvod ne postoji!");
                }
                else
                    return BadRequest("Ova prodavnica ne postoji!");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Proveri_dostupno/{IdProdavnice}/{Proizvod}")]
        public ActionResult Dostupno(int IdProdavnice, string Proizvod)
        {
            if (IdProdavnice < 0) return BadRequest("Prodavnica sa ovim Id-jem ne postoji!");
            if (Proizvod == "") return BadRequest("Morate uneti ime proizvoda!");

            try
            {
                var Shop = Context.Prodavnice
                                .Include(p => p.Meni)
                                .ThenInclude(p => p.Sastojci)
                                .Include(p => p.Frizider)
                                .Where(p => p.IDProdavnica == IdProdavnice)
                                .FirstOrDefault();

                if (Shop != null)
                {
                    foreach (var P in Shop.Meni)
                    {
                        if (P.Naziv == Proizvod)
                        {
                            var BrDostupnih = 1000;

                            foreach (var S in P.Sastojci)
                            {
                                foreach (var f in Shop.Frizider)
                                {
                                    if (f.Naziv == S.Naziv)
                                    {
                                        if (BrDostupnih > f.Kolicina / S.Kolicina)
                                        {
                                            BrDostupnih = f.Kolicina / S.Kolicina;
                                        }
                                    }
                                }
                            }

                            return Ok(BrDostupnih);
                        }
                    }

                    return BadRequest("Ovaj proizvod nema u ponudi!");
                }
                else
                    return BadRequest("Prodavnica sa ovim Id-em ne postoji!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}