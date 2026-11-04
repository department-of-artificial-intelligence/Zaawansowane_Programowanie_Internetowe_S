import { AutorService } from "../autorzy/AutorService";
import { WidokAutorow } from "../autorzy/WidokAutorow";
import type { Autor } from "../model/Autor";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

// Kod, który tylko wyświetla dane, dostaje je jako Readonly
function wypiszAutorow(autorzy: readonly Readonly<Autor>[], k: Konsola): void {
  for (const autor of autorzy) {
    k.log(`${autor.id}. ${autor.pelneImie}`);
  }
  // Celowa próba modyfikacji. Bez @ts-expect-error build zgłasza:
  // TS2540: Cannot assign to 'email' because it is a read-only property.
  // @ts-expect-error - Readonly<Autor> nie pozwala przypisać wartości
  if (autorzy.length < 0) autorzy[0].email = "zmiana@przyklad.pl";
  // TS2339: Property 'push' does not exist on type 'readonly Readonly<Autor>[]'.
  // @ts-expect-error - tablica tylko do odczytu nie ma push
  if (autorzy.length < 0) autorzy.push(autorzy[0]);
}

export const zadanie12: Zadanie = {
  numer: 12,
  tytul: "Readonly",
  async uruchom(kontener) {
    naglowek(kontener,
      "Lista z serwisu ma typ readonly Readonly<Autor>[]. Próby zmiany są w kodzie " +
      "(src/zadania/zad12.ts) oznaczone @ts-expect-error - po jego usunięciu kompilator zgłasza " +
      "TS2540 i TS2339.");
    const k = new Konsola(kontener);
    const serwis = new AutorService(ADRES_DANYCH);
    try {
      await serwis.wczytaj();
      const autorzy = serwis.pobierzWszystkich();
      wypiszAutorow(autorzy, k);
      new WidokAutorow(kontener, ["tabela"], "tabela").pokaz(autorzy);
    } catch (blad) {
      k.blad(blad);
    }
  },
};
