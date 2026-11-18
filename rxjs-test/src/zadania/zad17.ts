import { catchError, map, type Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import {
  obliczPole, pokazBlad, pokazWynik, utworzFormularz, wartosci$, type Formularz,
} from "./polePowierzchni";

// Zadanie 17: catchError zwraca strumień źródłowy (drugi parametr "caught"),
// więc po każdym błędzie strumień zostaje ponownie zasubskrybowany - bez limitu
class KalkulatorBezLimitu {
  private readonly f: Formularz;

  constructor(f: Formularz) {
    this.f = f;
  }

  start(): Subscription {
    return wartosci$(this.f)
      .pipe(
        map(obliczPole),
        catchError((e: Error, zrodlo$) => {
          pokazBlad(this.f, `${e.message}. Popraw wartość.`);
          return zrodlo$;
        }),
      )
      .subscribe(pole => pokazWynik(this.f, pole));
  }
}

export const zadanie17: Zadanie = {
  id: "17",
  tytul: "Poprawianie bez limitu (catchError)",
  opis: "catchError zwraca ten sam strumień, więc po błędzie aplikacja działa dalej, ile razy trzeba.",
  uruchom(kontener) {
    return new KalkulatorBezLimitu(utworzFormularz(kontener)).start();
  },
};
