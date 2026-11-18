import { fromEvent, map, merge, skipWhile, type Observable } from "rxjs";
import { akapit, etykieta, pole, wiersz } from "../ui";

// Wspólne elementy zadań 15–18: dwa pola tekstowe (boki a i b) i paragraf z wynikiem

// czy oba pola są wypełnione
export const czyNiepuste = ([a, b]: [string, string]): boolean =>
  a.trim() !== "" && b.trim() !== "";

export interface Formularz {
  bokA: HTMLInputElement;
  bokB: HTMLInputElement;
  wynik: HTMLParagraphElement;
  komunikat: HTMLParagraphElement;
}

export function utworzFormularz(kontener: HTMLElement): Formularz {
  const bokA = pole("text", "np. 4");
  const bokB = pole("text", "np. 2.5");
  const wynik = akapit();
  const komunikat = akapit();
  komunikat.classList.add("blad");
  const wskazowka = document.createElement("p");
  wskazowka.className = "opis";
  wskazowka.textContent = "Wartość zatwierdzasz Enterem albo przechodząc do drugiego pola (Tab).";
  kontener.append(wiersz(etykieta("Bok a: ", bokA), etykieta("Bok b: ", bokB)), wskazowka, wynik, komunikat);
  return { bokA, bokB, wynik, komunikat };
}

// Zatwierdzenie któregokolwiek pola (zdarzenie "change": Enter albo przejście do innego pola)
// emituje parę aktualnych wartości obu pól.
// - "change" zamiast "input": reagujemy na skończoną wartość, a nie na każdy klawisz
//   (inaczej np. samo "-" w trakcie wpisywania "-5" byłoby już błędem i zużywało próbę),
// - skipWhile: dopóki użytkownik nie wypełni obu pól, nic nie liczymy i nie zgłaszamy błędu,
//   bo formularz nie jest jeszcze gotowy. Po ponownej subskrypcji (retry) zasada działa od nowa,
// - merge zamiast combineLatest: po retry wystarczy poprawić jedno pole.
export function wartosci$(f: Formularz): Observable<[string, string]> {
  return merge(fromEvent(f.bokA, "change"), fromEvent(f.bokB, "change")).pipe(
    map(() => [f.bokA.value, f.bokB.value] as [string, string]),
    skipWhile(para => !czyNiepuste(para)),
  );
}

function naBok(tekst: string, nazwa: string): number {
  if (tekst.trim() === "") {
    throw new Error(`Pole "${nazwa}" jest puste`);
  }
  // przecinek też akceptujemy jako separator dziesiętny
  const liczba = Number(tekst.trim().replace(",", "."));
  if (!Number.isFinite(liczba)) {
    throw new Error(`Wartość "${tekst}" w polu "${nazwa}" nie jest liczbą`);
  }
  if (liczba < 0) {
    throw new Error(`Długość boku "${nazwa}" nie może być ujemna`);
  }
  return liczba;
}

// Rzuca wyjątek przy błędnych danych - w strumieniu RxJS zamienia się on w zdarzenie error
export function obliczPole([a, b]: [string, string]): number {
  return naBok(a, "bok a") * naBok(b, "bok b");
}

export function pokazWynik(f: Formularz, pole: number): void {
  f.wynik.textContent = `Pole = ${Number(pole.toFixed(6))}`;
  f.komunikat.textContent = "";
}

export function pokazBlad(f: Formularz, tekst: string): void {
  f.wynik.textContent = "";
  f.komunikat.textContent = tekst;
}

