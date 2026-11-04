import { AutorService } from "../autorzy/AutorService";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie10: Zadanie = {
  numer: 10,
  tytul: "Częściowa aktualizacja (Partial)",
  async uruchom(kontener) {
    naglowek(kontener,
      "aktualizuj(zmiany: Partial<NowyAutor>) - podajemy tylko zmieniane pola. " +
      "Walidacja obejmuje wszystkie pola przed zapisem, więc błąd niczego nie zmienia.");
    const k = new Konsola(kontener);
    const serwis = new AutorService(ADRES_DANYCH);
    try {
      await serwis.wczytaj();
      k.log("Przed:", String(serwis.znajdzPoId(1)));

      serwis.aktualizujAutora(1, { email: "nowy.adres@przyklad.pl" });
      k.log("Po zmianie samego e-maila:", String(serwis.znajdzPoId(1)));

      serwis.aktualizujAutora(1, { imie: "Adaś" });
      k.log("Po zmianie samego imienia:", String(serwis.znajdzPoId(1)));

      try {
        serwis.aktualizujAutora(1, { nazwisko: "Nowy", email: "zly-adres" });
      } catch (blad) {
        k.blad(blad);
      }
      k.log("Po nieudanej aktualizacji (nazwisko też bez zmian):", String(serwis.znajdzPoId(1)));

      k.log("Aktualizacja nieistniejącego autora:", serwis.aktualizujAutora(999, { imie: "X" }) ?? "undefined");
    } catch (blad) {
      k.blad(blad);
    }
  },
};
