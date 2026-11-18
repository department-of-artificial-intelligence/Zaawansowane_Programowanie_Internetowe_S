import { catchError, map, retry, throwError, type Subscription } from "rxjs";
import type { Zadanie } from "./Zadanie";
import {
  obliczPole, pokazBlad, pokazWynik, utworzFormularz, wartosci$, type Formularz,
} from "./polePowierzchni";

const LICZBA_POPRAWEK = 3;

// Zadanie 16: catchError pokazuje komunikat i przekazuje błąd dalej (throwError),
// retry(3) ponownie subskrybuje strumień - użytkownik może poprawić się 3 razy
class KalkulatorZPoprawkami {
  private readonly f: Formularz;
  private liczbaBledow = 0;

  constructor(f: Formularz) {
    this.f = f;
  }

  start(): Subscription {
    return wartosci$(this.f)
      .pipe(
        map(obliczPole),
        catchError((e: Error) => {
          this.liczbaBledow++;
          if (this.liczbaBledow <= LICZBA_POPRAWEK) {
            const zostalo = LICZBA_POPRAWEK - this.liczbaBledow + 1;
            pokazBlad(this.f, `Błąd ${this.liczbaBledow}: ${e.message}. ` +
              `Popraw wartość (pozostało prób: ${zostalo}).`);
          }
          return throwError(() => e);   // błąd idzie dalej, do retry
        }),
        retry(LICZBA_POPRAWEK),
      )
      .subscribe({
        next: pole => pokazWynik(this.f, pole),
        // tu trafiamy dopiero po 4. błędzie, gdy retry wyczerpie próby
        error: () => {
          pokazBlad(this.f, "Wprowadziłeś błędną wartość 4 razy. Koniec zabawy");
          this.f.bokA.disabled = this.f.bokB.disabled = true;
        },
      });
  }
}

export const zadanie16: Zadanie = {
  id: "16",
  tytul: "Trzy poprawki (catchError, retry, throwError)",
  opis: "Po błędzie można poprawić wartość. Czwarty błąd kończy działanie aplikacji.",
  uruchom(kontener) {
    return new KalkulatorZPoprawkami(utworzFormularz(kontener)).start();
  },
};
