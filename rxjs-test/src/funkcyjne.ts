// Zadania 5.7.1 – obsługa tablic w stylu funkcyjnym.
// Wszystkie funkcje są czyste: nie zmieniają tablicy wejściowej i nie mają efektów ubocznych.

export type Predykat<T> = (wartosc: T) => boolean;

// ---------- Zadanie 1: suma wszystkich elementów ----------
export const suma = (liczby: readonly number[]): number =>
  liczby.reduce((akumulator, liczba) => akumulator + liczba, 0);

// ---------- Zadanie 2: suma wartości parzystych ----------
export const czyParzysta: Predykat<number> = liczba => liczba % 2 === 0;

export const sumaParzystych = (liczby: readonly number[]): number =>
  suma(liczby.filter(czyParzysta));

// ---------- Zadanie 3: suma wartości większych od zadanej liczby ----------
export const sumaWiekszychNiz = (liczby: readonly number[], prog: number): number =>
  suma(liczby.filter(liczba => liczba > prog));

// Wspólny szkielet dla zadań 3–5: sumuj tylko to, co spełnia predykat
export const sumaGdzie = (liczby: readonly number[], warunek: Predykat<number>): number =>
  suma(liczby.filter(warunek));

// ---------- Zadanie 4: predykat jako osobna funkcja greaterThan ----------
// Najpierw próg, potem sprawdzana wartość - taka kolejność pozwala w zadaniu 5
// "zamrozić" próg przez currying i dostać gotowy predykat jednego argumentu.
export function greaterThan(prog: number, wartosc: number): boolean {
  return wartosc > prog;
}

export const sumaWiekszychNizZ4 = (liczby: readonly number[], prog: number): number =>
  sumaGdzie(liczby, liczba => greaterThan(prog, liczba));

// ---------- Zadanie 5: curry2 ----------
// Funkcja z punktu "Currying" (kod 5.17 w instrukcji): zamienia funkcję
// dwuparametrową fn(a1, a2) na ciąg funkcji jednoparametrowych fn(a1)(a2)
export function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
  return (a1: T1) => (a2: T2) => fn(a1, a2);
}

export const sumaWiekszychNizZ5 = (liczby: readonly number[], prog: number): number =>
  sumaGdzie(liczby, curry2(greaterThan)(prog));
// curry2(greaterThan)       : (prog: number) => (wartosc: number) => boolean
// curry2(greaterThan)(prog) : (wartosc: number) => boolean  <- gotowy predykat dla filter

// ---------- Zadanie 6: suma tych napisów, które reprezentują liczby ----------
// Number() zamiast parseFloat(): parseFloat("12abc") dałoby 12, a Number("12abc") daje NaN.
// Pusty napis i same spacje odrzucamy osobno, bo Number("") i Number("  ") dają 0.
export const czyLiczba: Predykat<string> = tekst =>
  tekst.trim() !== "" && Number.isFinite(Number(tekst));

export const sumaLiczbZNapisow = (napisy: readonly string[]): number =>
  napisy
    .filter(czyLiczba)
    .map(Number)
    .reduce((akumulator, liczba) => akumulator + liczba, 0);
