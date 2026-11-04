import { AutorService } from "../autorzy/AutorService";
import { WidokAutorow } from "../autorzy/WidokAutorow";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie06: Zadanie = {
  numer: 6,
  tytul: "Wyświetlenie autorów jako listy",
  async uruchom(kontener) {
    naglowek(kontener, "Pobrani autorzy wyświetleni na stronie jako lista.");
    const widok = new WidokAutorow(kontener, ["punktowana"], "punktowana");
    const serwis = new AutorService(ADRES_DANYCH);
    try {
      await serwis.wczytaj();
      widok.pokaz(serwis.pobierzWszystkich());
    } catch (blad) {
      new Konsola(kontener).blad(blad);
    }
  },
};
