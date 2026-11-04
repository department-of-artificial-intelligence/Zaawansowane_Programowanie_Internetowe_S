import { AutorService } from "../autorzy/AutorService";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie11: Zadanie = {
  numer: 11,
  tytul: "Record zamiast tablicy",
  async uruchom(kontener) {
    naglowek(kontener,
      "AutorService trzyma autorów w Record<number, Autor>. Wyszukiwanie to jeden odczyt " +
      "po kluczu (stały czas), a w tablicy find() przegląda elementy po kolei (czas liniowy).");
    const k = new Konsola(kontener);
    const serwis = new AutorService(ADRES_DANYCH);
    try {
      await serwis.wczytaj();
      k.log("znajdzPoId(4):", String(serwis.znajdzPoId(4)));

      // porównanie na dużej liczbie elementów
      const ile = 200_000;
      const tablica = Array.from({ length: ile }, (_, i) => ({ id: i, nazwa: `autor ${i}` }));
      const slownik: Record<number, { id: number; nazwa: string }> = {};
      for (const element of tablica) slownik[element.id] = element;

      const szukane = Array.from({ length: 1_000 }, () => Math.floor(Math.random() * ile));
      let start = performance.now();
      for (const id of szukane) tablica.find(e => e.id === id);
      const czasTablicy = performance.now() - start;

      start = performance.now();
      for (const id of szukane) void slownik[id];
      const czasRecord = performance.now() - start;

      k.log(`1000 wyszukiwań wśród ${ile} elementów:`);
      k.log(`  tablica.find():  ${czasTablicy.toFixed(2)} ms`);
      k.log(`  record[id]:      ${czasRecord.toFixed(2)} ms`);
    } catch (blad) {
      k.blad(blad);
    }
  },
};
