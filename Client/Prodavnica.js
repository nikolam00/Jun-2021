import { Sastojak } from "./Sastojak.js";

export class Prodavnica {
    constructor(id, Naziv, Meni, Frizider) {
        this.id = id;
        this.Naziv = Naziv;
        this.Meni = Meni;
        this.Frizider = Frizider;

        this.Nedostaje = [];

        this.kontatiner = null;
    }

    removeAllChildNodes(parent) {
        while (parent.firstChild) {
            parent.removeChild(parent.firstChild);
        }
    }

    crtaj(host) {
        this.kontatiner = document.createElement("div");
        this.kontatiner.classList.add("glavniKontejner");
        host.appendChild(this.kontatiner);

        var Zaglavlje = document.createElement("div");
        Zaglavlje.className = "Zaglavlje";
        this.kontatiner.appendChild(Zaglavlje);

        var Prozori = document.createElement("div");
        Prozori.className = "Prozori";
        this.kontatiner.appendChild(Prozori);

        this.crtajZaglavlje(Zaglavlje);
        this.crtajProzore(Prozori);
    }

    crtajZaglavlje(host) {
        this.removeAllChildNodes(host);

        var Naslov = document.createElement("div");
        Naslov.className = "Naslov";
        host.appendChild(Naslov);

        var lblNaslov = document.createElement("label");
        lblNaslov.innerHTML = this.Naziv;
        Naslov.appendChild(lblNaslov);
    }

    crtajProzore(host) {
        this.removeAllChildNodes(host);

        var LeviProzor = document.createElement("div");
        LeviProzor.className = "LeviProzor";
        host.appendChild(LeviProzor);

        var DesniProzor = document.createElement("div");
        DesniProzor.className = "DesniProzor";
        host.appendChild(DesniProzor);

        this.crtajLeviProzor(LeviProzor);
        this.crtajDesniProzor(DesniProzor);
    }

    crtajLeviProzor(host) {
        this.removeAllChildNodes(host);

        var header = document.createElement("h2");
        header.innerHTML = "PORUCIVANJE";
        host.appendChild(header);

        var Narudzbina = document.createElement("div");
        Narudzbina.className = "Narudzbina";
        host.appendChild(Narudzbina);

        var Proizvod = document.createElement("div");
        Proizvod.className = "divProizvod";
        Narudzbina.host(Proizvod);

        var lblProizvod = document.createElement("label");
        lblProizvod.innerHTML = "Proizvod";
        Proizvod.appendChild(lblProizvod);

        var selectProizvod = document.createElement("select");
        selectProizvod.className = "selectProizvod";
        var Option;
        this.Meni.forEach(P => {
            Option = document.createElement("option");
            Option.value = P.Naziv;
            Option.innerHTML = P.Naziv;
            selectProizvod.appendChild(Option);
        });
        Proizvod.appendChild(selectProizvod);

        var Kolicina = document.createElement("input");
        Kolicina.type = "inputKolicina";
        Narudzbina.appendChild(Kolicina);

        var btnPoruci = document.createElement("button");
        btnPoruci.innerHTML = "Poruci";
        btnPoruci.onclick = (ev) => Naruci(selectProizvod.value, Kolicina.value);

        host.appendChild(btnPoruci);
    }

    crtajDesniProzor(host) {
        this.removeAllChildNodes(host);

        var header = document.createElement("h2");
        header.innerHTML = "Nabavka";
        host.appendChild(header);

        var divNedostaje = document.createElement("div");

        this.Nedostaje.forEach(p => {
            var Stavka = document.createElement("div");
            Stavka.className = "divStavka";
            divNedostaje.appendChild(Stavka);

            var cb = document.createElement("checkbox");
            cb.className = "cb";
            cb.value = p.Naziv;
            Stavka.appendChild(cb);

            var lblStavka = document.createElement("label")
            lblStavka.innerHTML = p.Naziv;
            Stavka.appendChild(lblStavka);

        });

        host.appendChild(divNedostaje);

        var btnDostavi = document.createElement("button");
        btnDostavi.innerHTML = "Dostavi";
        btnDostavi.onclick = (ev) => this.Dostavi();
        host.appendChild(btnDostavi);
    }

    Naruci(Proizvod, Kolicina) {
        fetch("https://localhost:5001/Prodavnica/Naruci_proizvod/" + this.id + "/" + Proizvod + "/" + Kolicina, {
            method: 'PUT',
            body: JSON.stringify({
                "IdProdavnice": this.id,
                "Proizvod": Proizvod,
                "brNarudzbina": Kolicina
            })
        }).then(R => {
            if (R === null) {
                alert("Vasa narudzbina je uspesno izvrsena!");
                let LeviProzor = this.kontatiner.querySelector(".leviProzor");
                this.crtajLeviProzor(LeviProzor);
            } else {
                alert("Vasa narudzbina nazalost nije izvrsena!");
                fetch("https://localhost:5001/Prodavnica/Proveri_dostupno/" + this.id + "/" + Proizvod)
                    .then(p => {
                        p.json().then(D => {
                            alert("Trentuno mozete naruciti " + D + " " + Proizvod);
                        });
                    })

                R.forEach(S => {
                    var Sastojak = new Sastojak(S.Naziv, S.Kolicina);
                    this.Nedostaje.push(Sastojak);
                })

                let DesniProzor = this.kontatiner.querySelector("DesniProzor");
                this.crtajDesniProzor(DesniProzor);
            }
        })
    }

    Dostavi() {

        this.kontatiner.querySelectorAll(".cb")

    }
}