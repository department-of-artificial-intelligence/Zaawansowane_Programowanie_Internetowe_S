import type { Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import { przesun, ruchMyszy$, utworzKropke } from "./kursor";

// Zadanie 19: div podąża za kursorem
class SledzenieKursora {
  private readonly kropka: HTMLDivElement;

  constructor(kropka: HTMLDivElement) {
    this.kropka = kropka;
  }

  start(): Subscription {
    return ruchMyszy$.subscribe(p => przesun(this.kropka, p));
  }
}

export const zadanie19: Zadanie = {
  id: "19",
  tytul: "Div śledzi kursor",
  opis: "Porusz myszą – kolorowy div podąża za kursorem (fromEvent na zdarzeniu mousemove).",
  uruchom(kontener) {
    return new SledzenieKursora(utworzKropke(kontener, "#e8590c")).start();
  },
};
