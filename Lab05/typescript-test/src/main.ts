const liczby: number[] = [5, 10, 15, 20, 25];

// zad 1
/*

const suma = liczby
  .reduce((akumulator, liczba) => {
    return akumulator + liczba;
}, 0);

console.log("Tablica liczb:", liczby);
console.log("Suma wszystkich elementów:", suma);
*/

// zad 2
/*
const suma = liczby
  .filter((liczba, indeks) => indeks % 2 === 0)
  .reduce((akumulator, liczba) => {
    return akumulator + liczba;
}, 0);

console.log("Tablica liczb:", liczby);
console.log("Suma co drugiego elementu:", suma);


*/

const zadanaLiczba: number = 12;
// zad 3
/*

const suma = liczby
  .filter(liczba => liczba > zadanaLiczba)
  .filter((liczba, indeks) => indeks % 2 === 0)
  .reduce((akumulator, liczba) => {
    return akumulator + liczba;
}, 0);

console.log("Tablica liczb:", liczby);
console.log("suma co drugiego elementu wiekszego niz zadana liczba:", suma);
*/

let greaterThan : (liczba:number, liczba2:number) => boolean;

greaterThan = function (zadanaLiczba:number, liczba:number):boolean{
  return liczba > zadanaLiczba;
}

// zad 4
/*
const suma = liczby
  .filter(liczba => greaterThan(zadanaLiczba, liczba))
  .filter((liczba, indeks) => indeks % 2 === 0)
  .reduce((akumulator, liczba) => {
    return akumulator + liczba;
}, 0);

console.log("Tablica liczb:", liczby);
console.log("suma co drugiego elementu wiekszego niz zadana liczba:", suma);
*/


// zad 5
/*
function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
  return (a1: T1) => (a2: T2) => fn(a1, a2);
}

const curriedGreaterThan = curry2(greaterThan);

const wiekszeOd12 = curriedGreaterThan(12);

const suma = liczby
  .filter(wiekszeOd12)
  .filter((liczba, indeks) => indeks % 2 === 0)
  .reduce((akumulator, liczba) => akumulator + liczba, 0);
console.log("Tablica liczb:", liczby);
console.log("Suma:", suma);
*/

// zad 6
/*
const teksty: string[] = ["Ala", "1", "Ewa", "12.4"];

const suma = teksty
  .filter(tekst => tekst.trim() !== "" && !isNaN(Number(tekst)))
  .map(tekst => Number(tekst))
  .reduce((akumulator, liczba) => akumulator + liczba, 0);

console.log("Suma liczb:", suma);
*/

// zad 7

import { fromEvent } from "rxjs";
import type { Observer } from "rxjs";

/*
class ClickObserver implements Observer<Event> {
    private liczbaKlikniec: number = 0;
    private id: string;

    constructor(id: string) {
        this.id = id;
    }

    next(value: Event): void {
        this.liczbaKlikniec++;

        document.querySelector(`#${this.id}`)!.innerHTML =
            `Liczba kliknięć: ${this.liczbaKlikniec}`;
    }

    error(err: any): void {
        console.log("Błąd:", err);
    }

    complete(): void {
        console.log("Zakończono");
    }
}

let element = document.querySelector<HTMLButtonElement>("#button")!;

fromEvent(element, "click").subscribe(new ClickObserver("info"));
*/



import { map } from "rxjs";
import { scan } from "rxjs";


// zad 9
/*
class ClickObserver {
    private id: string;

    constructor(id: string) {
        this.id = id;
    }

    start(): void {
        const element = document.querySelector<HTMLButtonElement>("#button")!;

        fromEvent(element, "click")
            .pipe(
                map(() => 1),
                scan((akumulator, wartosc) => akumulator + wartosc, 0)
            )
            .subscribe(liczbaKlikniec => {
                document.querySelector(`#${this.id}`)!.innerHTML =
                    `Liczba kliknięć: ${liczbaKlikniec}`;
            });
    }
}

const observer = new ClickObserver("info");
observer.start();
*/

// zadanie 9
import { take } from "rxjs";

/*
class ClickObserver {
    private id: string;

    constructor(id: string) {
        this.id = id;
    }

    start(): void {
        const element = document.querySelector<HTMLButtonElement>("#button")!;

        fromEvent(element, "click")
            .pipe(
                take(5),
                map(() => 1),
                scan((akumulator, wartosc) => akumulator + wartosc, 0)
            )
            .subscribe(liczbaKlikniec => {
                document.querySelector(`#${this.id}`)!.innerHTML =
                    `Liczba kliknięć: ${liczbaKlikniec}`;
            });
    }
}

const observer = new ClickObserver("info");
observer.start();
*/

// zadanie 10