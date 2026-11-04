import { EksportAutorow, FORMATY } from "../formatowanie/EksportAutorow";
import { Autor } from "../model/Autor";
import { naglowek, type Zadanie } from "./Zadanie";

export const zadanie03: Zadanie = {
  numer: 3,
  tytul: "Różne formaty autorów (Strategia)",
  uruchom(kontener) {
    naglowek(kontener,
      "Wzorzec Strategia: każdy format to klasa implementująca FormatAutorow. " +
      "EksportAutorow dostaje strategię i może ją zmienić w trakcie działania.");

    const autorzy = [
      new Autor("Ala", "Kowalska", "ala@przyklad.pl"),
      new Autor("Jan", "Nowak", "jan@przyklad.pl"),
    ];
    const eksport = new EksportAutorow(FORMATY.tabela);

    for (const format of Object.values(FORMATY)) {
      eksport.ustawFormat(format);
      const wynik = eksport.eksportuj(autorzy);

      const sekcja = document.createElement("section");
      const tytul = document.createElement("h3");
      tytul.textContent = `${format.nazwa} (${format.typMime})`;
      sekcja.appendChild(tytul);

      if (format.typMime === "text/html") {
        const podglad = document.createElement("div");
        podglad.className = "wynik";
        podglad.innerHTML = wynik;
        sekcja.appendChild(podglad);
      }
      const kod = document.createElement("pre");
      kod.className = "konsola";
      kod.textContent = wynik;
      sekcja.appendChild(kod);
      kontener.appendChild(sekcja);
    }
  },
};
