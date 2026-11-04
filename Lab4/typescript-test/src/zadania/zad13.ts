import { AutorService } from "../autorzy/AutorService";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie13: Zadanie = {
  numer: 13,
  tytul: "Wyszukiwanie z ?. i ??",
  async uruchom(kontener) {
    naglowek(kontener,
      "znajdzPoId zwraca undefined, gdy autora nie ma. Wynik wyświetla jedno wyrażenie: " +
      "autor?.pelneImie ?? \"Nie znaleziono autora\" - bez instrukcji if.");
    const k = new Konsola(kontener);
    const serwis = new AutorService(ADRES_DANYCH);

    const pole = document.createElement("input");
    pole.type = "number";
    pole.value = "3";
    const przycisk = document.createElement("button");
    przycisk.type = "button";
    przycisk.textContent = "Szukaj";
    const wynik = document.createElement("strong");
    const wiersz = document.createElement("div");
    wiersz.className = "licznik";
    wiersz.append("Id autora: ", pole, przycisk, " ", wynik);
    kontener.insertBefore(wiersz, kontener.lastChild);

    const opisAutora = (id: number): string =>
      serwis.znajdzPoId(id)?.pelneImie ?? "Nie znaleziono autora";

    try {
      await serwis.wczytaj();
      przycisk.addEventListener("click", () => { wynik.textContent = opisAutora(Number(pole.value)); });
      wynik.textContent = opisAutora(Number(pole.value));
      k.log("id = 1   ->", opisAutora(1));
      k.log("id = 999 ->", opisAutora(999));
    } catch (blad) {
      k.blad(blad);
    }
  },
};
