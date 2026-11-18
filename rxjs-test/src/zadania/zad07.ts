import { fromEvent, tap, type Subscription } from "rxjs";
import { akapit, przycisk } from "../ui";
import type { Zadanie } from "./Zadanie";

// Zadanie 7: liczba kliknięć przechowywana w polu klasy, zwiększana operatorem tap
class LicznikKlikniec {
  private liczbaKlikniec = 0;
  private readonly przycisk: HTMLButtonElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(przycisk: HTMLButtonElement, akapit: HTMLParagraphElement) {
    this.przycisk = przycisk;
    this.akapit = akapit;
  }

  start(): Subscription {
    this.pokaz();
    return fromEvent(this.przycisk, "click")
      .pipe(tap(() => this.liczbaKlikniec++))   // efekt uboczny: zmiana pola klasy
      .subscribe(() => this.pokaz());
  }

  private pokaz(): void {
    this.akapit.textContent = `Przycisk kliknięto ${this.liczbaKlikniec} razy`;
  }
}

export const zadanie07: Zadanie = {
  id: "7",
  tytul: "Licznik kliknięć z polem klasy (tap)",
  opis: "Pole liczbaKlikniec zwiększa operator tap, a subscribe wyświetla jego wartość.",
  uruchom(kontener) {
    const b = przycisk("Kliknij mnie");
    const p = akapit();
    kontener.append(b, p);
    return new LicznikKlikniec(b, p).start();
  },
};
