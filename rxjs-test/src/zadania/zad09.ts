import { fromEvent, map, scan, startWith, take, type Subscription } from "rxjs";
import { akapit, przycisk } from "../ui";
import type { Zadanie } from "./Zadanie";

const LIMIT = 5;
const opis = (n: number) => `Przycisk kliknięto ${n} z ${LIMIT} razy`;

// Zadanie 9: take(5) przepuszcza tylko 5 pierwszych kliknięć, potem strumień się kończy
class OgraniczonyLicznik {
  private readonly przycisk: HTMLButtonElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(przycisk: HTMLButtonElement, akapit: HTMLParagraphElement) {
    this.przycisk = przycisk;
    this.akapit = akapit;
  }

  start(): Subscription {
    return fromEvent(this.przycisk, "click")
      .pipe(
        take(LIMIT),
        scan(liczba => liczba + 1, 0),
        map(opis),
        startWith(opis(0)),
      )
      .subscribe({
        next: tekst => (this.akapit.textContent = tekst),
        // complete wywoła się po 5. kliknięciu
        complete: () => {
          this.akapit.textContent += " – limit wyczerpany";
          this.przycisk.disabled = true;
        },
      });
  }
}

export const zadanie09: Zadanie = {
  id: "9",
  tytul: "Maksymalnie 5 kliknięć (take)",
  opis: "Po 5. kliknięciu take kończy strumień (complete), a przycisk zostaje zablokowany.",
  uruchom(kontener) {
    const b = przycisk("Kliknij mnie");
    const p = akapit();
    kontener.append(b, p);
    return new OgraniczonyLicznik(b, p).start();
  },
};
