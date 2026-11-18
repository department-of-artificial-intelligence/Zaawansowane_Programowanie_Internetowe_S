import { fromEvent, map, type Subscription } from "rxjs";
import { akapit, pole } from "../ui";
import type { Zadanie } from "./Zadanie";

// Zadanie 12: zawartość pola przepisywana do paragrafu przy każdej zmianie
class PodgladTekstu {
  private readonly pole: HTMLInputElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(pole: HTMLInputElement, akapit: HTMLParagraphElement) {
    this.pole = pole;
    this.akapit = akapit;
  }

  start(): Subscription {
    return fromEvent(this.pole, "input")
      .pipe(map(() => this.pole.value))
      .subscribe(tekst => (this.akapit.textContent = tekst));
  }
}

export const zadanie12: Zadanie = {
  id: "12",
  tytul: "Tekst z pola w paragrafie",
  opis: "Zdarzenie input zamieniane jest operatorem map na aktualną wartość pola.",
  uruchom(kontener) {
    const i = pole("text", "Wpisz tekst");
    const p = akapit();
    kontener.append(i, p);
    return new PodgladTekstu(i, p).start();
  },
};
