import { map, type Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import {
  obliczPole, pokazBlad, pokazWynik, utworzFormularz, wartosci$, type Formularz,
} from "./polePowierzchni";

// Zadanie 15: błąd przechwytywany dopiero w subscribe
class KalkulatorPola {
  private readonly f: Formularz;

  constructor(f: Formularz) {
    this.f = f;
  }

  start(): Subscription {
    return wartosci$(this.f)
      .pipe(map(obliczPole))
      .subscribe({
        next: pole => pokazWynik(this.f, pole),
        error: (e: Error) => {
          pokazBlad(this.f, `${e.message}. Strumień zakończył się błędem – ` +
            "kolejne zmiany w polach nie są już obsługiwane.");
          this.f.bokA.disabled = this.f.bokB.disabled = true;
        },
      });
  }
}

export const zadanie15: Zadanie = {
  id: "15",
  tytul: "Pole powierzchni – błąd w subscribe",
  opis: "Wpisz dwie liczby – pojawi się pole. Potem wpisz coś, co liczbą nie jest (albo wyczyść pole). " +
    "Po pierwszym błędzie strumień kończy działanie i poprawienie wartości niczego już nie zmienia.",
  uruchom(kontener) {
    return new KalkulatorPola(utworzFormularz(kontener)).start();
  },
};
