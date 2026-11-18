import { combineLatest, filter, fromEvent, map, startWith, type Subscription } from "rxjs";
import { akapit, etykieta, pole, wiersz } from "../ui";
import type { Zadanie } from "./Zadanie";

// Wartość pola number jako liczba albo undefined, gdy to nie jest liczba
export function naLiczbe(tekst: string): number | undefined {
  if (tekst.trim() === "") return undefined;
  const liczba = Number(tekst);
  return Number.isFinite(liczba) ? liczba : undefined;
}

// Zadanie 14: próg długości pochodzi z drugiego pola; łączymy oba strumienie combineLatest
class PodgladZProgiem {
  private readonly pole: HTMLInputElement;
  private readonly poleProgu: HTMLInputElement;
  private readonly akapit: HTMLParagraphElement;

  constructor(pole: HTMLInputElement, poleProgu: HTMLInputElement, akapit: HTMLParagraphElement) {
    this.pole = pole;
    this.poleProgu = poleProgu;
    this.akapit = akapit;
  }

  start(): Subscription {
    // startWith: combineLatest emituje dopiero, gdy KAŻDE źródło coś wyemitowało,
    // więc dajemy obu strumieniom wartości początkowe z pól
    const tekst$ = fromEvent(this.pole, "input").pipe(
      map(() => this.pole.value),
      startWith(this.pole.value),
    );
    const prog$ = fromEvent(this.poleProgu, "input").pipe(
      map(() => naLiczbe(this.poleProgu.value)),
      startWith(naLiczbe(this.poleProgu.value)),
    );

    return combineLatest([tekst$, prog$])
      .pipe(
        // próg nie jest liczbą -> przepuszczamy cały tekst
        filter(([tekst, prog]) => prog === undefined || tekst.length > prog),
        map(([tekst]) => tekst),
      )
      .subscribe(tekst => (this.akapit.textContent = tekst));
  }
}

export const zadanie14: Zadanie = {
  id: "14",
  tytul: "Próg długości z drugiego pola (combineLatest)",
  opis: "Tekst pojawia się, gdy jest dłuższy niż liczba z drugiego pola. Gdy drugie pole " +
    "nie zawiera liczby (np. jest puste), wyświetla się cały tekst.",
  uruchom(kontener) {
    const i = pole("text", "Wpisz tekst");
    const prog = pole("number");
    prog.value = "5";
    const p = akapit();
    kontener.append(wiersz(etykieta("Tekst: ", i), etykieta("Minimalna długość: ", prog)), p);
    return new PodgladZProgiem(i, prog, p).start();
  },
};
