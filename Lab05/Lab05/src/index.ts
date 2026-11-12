import { fromEvent } from "rxjs";
import { tap } from "rxjs/operators";

//-----------------------
//TYPESCRIPT W STYLU FUNKCYJNYM

const numbers: number[] = [1, 2, 3, 4, 5, 6];
console.log("Tablica:", numbers);

//ZADANIE 1
const sum: number = numbers.reduce((acc: number, curr: number) => acc + curr, 0);
console.log("Zadanie 1 (suma elementow):", sum);

//ZADANIE 2
const evenSum: number = numbers
    .filter((n: number) => n % 2 === 0)
    .reduce((acc: number, curr: number) => acc + curr, 0);

console.log("Zadanie 2 (suma liczb parzystych):", evenSum);

//ZADANIE 3
const threshold: number = 3;

const sumGreaterThan: number = numbers
    .filter((n: number) => n > threshold)
    .reduce((acc: number, curr: number) => acc + curr, 0);

console.log(`Zadanie 3 (suma liczb większych od ${threshold}:)`, sumGreaterThan);

//ZADANIE 4

// Funkcja wyższego rzędu zwracająca predykat
function greaterThan(threshold: number): (value: number) => boolean {
    return (value: number): boolean => value > threshold;
}

const sumGreaterThanv2: number = numbers
    .filter(greaterThan(threshold))
    .reduce((acc: number, curr: number) => acc + curr, 0);

console.log(`Zadanie 4 (suma liczb większych od ${threshold}:)`, sumGreaterThanv2);

//ZADANIE 5
function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
    return (a1: T1) => (a2: T2) => fn(a1, a2);
}

// Zwykła funkcja dwuargumentowa porównująca dwie liczby
const isGreater = (threshold: number, value: number): boolean => value > threshold;

// Przekształcenie funkcji dwuargumentowej w ciąg funkcji jednoargumentowych
const greaterThanv2 = curry2(isGreater);

const sumGreaterThanv3: number = numbers
    .filter(greaterThanv2(threshold))
    .reduce((acc: number, curr: number) => acc + curr, 0);

console.log(`Zadanie 5 (suma liczb większych od ${threshold}:)`, sumGreaterThanv3);

//--------------

const items: string[] = ["Ala", "1", "Ewa", "12.4"];
console.log("Tablica2:", items);

//ZADANIE 6
const sumOfNumbers: number = items
    .map((item: string) => Number(item))          // Zamiana tekstu na liczbę (Ala -> NaN)
    .filter((num: number) => !Number.isNaN(num))  // Odrzucenie wszystkiego co nie jest liczbą
    .reduce((acc: number, curr: number) => acc + curr, 0);

console.log("Zadanie 6 (suma liczb:)", sumOfNumbers);


//-------------------------
//ZADANIA Z BIBLIOTEKI RXJS

//ZADANIE 7

class ClickCounterApp {
    private button: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;
    private count: number = 0;

    constructor(buttonId: string, paragraphId: string) {
        this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        fromEvent(this.button, "click")
            .pipe(
                tap(() => {
                    this.count++;
                })
            )
            .subscribe(() => {
                this.paragraph.textContent = `Liczba kliknięć: ${this.count}`;
            });
    }
}

const app = new ClickCounterApp("clickBtn", "counterOutput");
app.init();