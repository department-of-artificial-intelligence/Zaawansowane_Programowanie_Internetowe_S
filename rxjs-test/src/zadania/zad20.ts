import { delay, Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import { przesun, ruchMyszy$, utworzKropke } from "./kursor";

const OPOZNIENIE_MS = 100;

// Zadanie 20: ten sam strumień, ale każde położenie dociera do diva 100 ms później
class SledzenieZOpoznieniem {
  private readonly kropka: HTMLDivElement;
  private readonly kropkaBezOpoznienia: HTMLDivElement;

  constructor(kropka: HTMLDivElement, kropkaBezOpoznienia: HTMLDivElement) {
    this.kropka = kropka;
    this.kropkaBezOpoznienia = kropkaBezOpoznienia;
  }

  start(): Subscription {
    const wszystko = new Subscription();
    wszystko.add(ruchMyszy$
      .pipe(delay(OPOZNIENIE_MS))
      .subscribe(p => przesun(this.kropka, p)));
    // dla porównania: mała kropka bez opóźnienia
    wszystko.add(ruchMyszy$.subscribe(p => przesun(this.kropkaBezOpoznienia, p)));
    return wszystko;
  }
}

export const zadanie20: Zadanie = {
  id: "20",
  tytul: "Div śledzi kursor z opóźnieniem 100 ms (delay)",
  opis: "Duży div dostaje położenie kursora z opóźnieniem 100 ms. Mała szara kropka " +
    "(bez opóźnienia) pokazuje różnicę.",
  uruchom(kontener) {
    const opozniona = utworzKropke(kontener, "#1971c2");
    const bezOpoznienia = utworzKropke(kontener, "#868e96");
    bezOpoznienia.classList.add("mala");
    return new SledzenieZOpoznieniem(opozniona, bezOpoznienia).start();
  },
};
