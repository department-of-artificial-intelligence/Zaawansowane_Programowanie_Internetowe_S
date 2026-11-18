import { catchError, filter, map, type Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import {
  czyNiepuste, obliczPole, pokazBlad, pokazWynik, utworzFormularz, wartosci$, type Formularz,
} from "./polePowierzchni";

// Zadanie 18: filter odrzuca stan, w którym któreś pole jest puste,
// więc błąd pojawia się tylko wtedy, gdy wartość nie jest liczbą
class KalkulatorBezPustych {
  private readonly f: Formularz;

  constructor(f: Formularz) {
    this.f = f;
  }

  start(): Subscription {
    return wartosci$(this.f)
      .pipe(
        filter(czyNiepuste),
        map(obliczPole),
        catchError((e: Error, zrodlo$) => {
          pokazBlad(this.f, `${e.message}. Popraw wartość.`);
          return zrodlo$;
        }),
      )
      .subscribe(pole => pokazWynik(this.f, pole));
  }
}

export const zadanie18: Zadanie = {
  id: "18",
  tytul: "Bez reakcji na puste pole (filter)",
  opis: "W zadaniu 17 wyczyszczenie pola po pierwszym obliczeniu daje komunikat o błędzie. " +
    "Tutaj filter pomija puste pole – komunikat pojawia się tylko, gdy wartość nie jest liczbą.",
  uruchom(kontener) {
    return new KalkulatorBezPustych(utworzFormularz(kontener)).start();
  },
};
