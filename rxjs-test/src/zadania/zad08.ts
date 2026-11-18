import { fromEvent, map, scan, startWith, type Subscription } from "rxjs";
import { akapit, przycisk } from "../ui";
import type { Zadanie } from "./Zadanie";

const opis = (n: number) => `Przycisk kliknięto ${n} razy`;

// Zadanie 8: bez pola - stan (liczba kliknięć) przechowuje operator scan
class LicznikKlikniecScan {
  private readonly przycisk: HTMLButtonElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(przycisk: HTMLButtonElement, akapit: HTMLParagraphElement) {
    this.przycisk = przycisk;
    this.akapit = akapit;
  }

  start(): Subscription {
    return fromEvent(this.przycisk, "click")
      .pipe(
        scan(liczba => liczba + 1, 0),  // 1, 2, 3, ...
        map(opis),
        startWith(opis(0)),             // tekst przed pierwszym kliknięciem
      )
      .subscribe(tekst => (this.akapit.textContent = tekst));
  }
}

export const zadanie08: Zadanie = {
  id: "8",
  tytul: "Licznik kliknięć bez pola (scan, map)",
  opis: "scan sumuje kliknięcia, map zamienia liczbę na tekst. Klasa nie ma pola z licznikiem.",
  uruchom(kontener) {
    const b = przycisk("Kliknij mnie");
    const p = akapit();
    kontener.append(b, p);
    return new LicznikKlikniecScan(b, p).start();
  },
};
