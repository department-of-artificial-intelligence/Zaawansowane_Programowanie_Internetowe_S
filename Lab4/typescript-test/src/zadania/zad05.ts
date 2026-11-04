import { AutorService } from "../autorzy/AutorService";
import { Autor } from "../model/Autor";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie05: Zadanie = {
  numer: 5,
  tytul: "Pobranie autorów z serwera",
  async uruchom(kontener) {
    naglowek(kontener, `Asynchroniczne pobranie ${ADRES_DANYCH} i zamiana danych na obiekty klasy Autor.`);
    const k = new Konsola(kontener);
    const serwis = new AutorService(ADRES_DANYCH);

    try {
      const wynik = await serwis.wczytaj();
      k.log(`Wczytano autorów: ${wynik.wczytano}`);
      for (const autor of serwis.pobierzWszystkich()) {
        k.log(`  #${autor.id} ${autor}  (instanceof Autor: ${autor instanceof Autor})`);
      }
      wynik.bledy.forEach(blad => k.blad(`Pominięto: ${blad}`));
    } catch (blad) {
      k.blad(blad);
    }
  },
};
