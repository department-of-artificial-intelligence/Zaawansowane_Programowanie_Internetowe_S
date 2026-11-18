import { filter, fromEvent, map, type Subscription } from "rxjs";
import { akapit, pole } from "../ui";
import type { Zadanie } from "./Zadanie";

const MIN_DLUGOSC = 5;

// Zadanie 13: do paragrafu trafia tylko tekst dłuższy niż 5 znaków
class PodgladDlugiegoTekstu {
  private readonly pole: HTMLInputElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(pole: HTMLInputElement, akapit: HTMLParagraphElement) {
    this.pole = pole;
    this.akapit = akapit;
  }

  start(): Subscription {
    return fromEvent(this.pole, "input")
      .pipe(
        map(() => this.pole.value),
        filter(tekst => tekst.length > MIN_DLUGOSC),
      )
      .subscribe(tekst => (this.akapit.textContent = tekst));
  }
}

export const zadanie13: Zadanie = {
  id: "13",
  tytul: "Tylko tekst dłuższy niż 5 znaków (filter)",
  opis: "filter zatrzymuje krótsze teksty, więc paragraf pokazuje ostatni tekst, który przeszedł warunek.",
  uruchom(kontener) {
    const i = pole("text", "Wpisz więcej niż 5 znaków");
    const p = akapit();
    kontener.append(i, p);
    return new PodgladDlugiegoTekstu(i, p).start();
  },
};
