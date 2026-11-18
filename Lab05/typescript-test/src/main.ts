import { fromEvent, map, scan, take, Subscription, shareReplay, filter, combineLatest, startWith, throwError, catchError, retry, mergeMap, of, withLatestFrom, delay} from "rxjs";
import type { Observer } from "rxjs";
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

/*

class ClickObserver {
    private subscription?: Subscription;
    private liczbaKlikniec: number = 0;

    start(): void {
        const buttonStart = document.querySelector<HTMLButtonElement>("#start")!;
        const buttonStop = document.querySelector<HTMLButtonElement>("#stop")!;
        const buttonClick = document.querySelector<HTMLButtonElement>("#button")!;

        fromEvent(buttonStart, "click").subscribe(() => {
            if (!this.subscription || this.subscription.closed) {
                this.subscription = fromEvent(buttonClick, "click")
                    .subscribe(() => {
                        this.liczbaKlikniec++;

                        document.querySelector("#info")!.innerHTML =
                            `Liczba kliknięć: ${this.liczbaKlikniec}`;
                    });
            }
        });

        fromEvent(buttonStop, "click").subscribe(() => {
            this.subscription?.unsubscribe();
        });
    }
}

const observer = new ClickObserver();
observer.start();

*/

// zadanie 11
/*
class ClickObserver {
    private subscription1?: Subscription;
    private subscription2?: Subscription;

    start(): void {
        const buttonClick = document.querySelector<HTMLButtonElement>("#button")!;
        const button1 = document.querySelector<HTMLButtonElement>("#button1")!;
        const button2 = document.querySelector<HTMLButtonElement>("#button2")!;

        const input1 = document.querySelector<HTMLInputElement>("#input1")!;
        const input2 = document.querySelector<HTMLInputElement>("#input2")!;

        const licznik$ = fromEvent(buttonClick, "click").pipe(
            map(() => 1),
            scan((akumulator, wartosc) => akumulator + wartosc, 0),
            shareReplay({ bufferSize: 1, refCount: false })
        );

        licznik$.subscribe();

        fromEvent(button1, "click").subscribe(() => {
            if (!this.subscription1 || this.subscription1.closed) {
                this.subscription1 = licznik$.subscribe(liczba => {
                    input1.value = liczba.toString();
                });
            } else {
                this.subscription1.unsubscribe();
            }
        });
        fromEvent(button2, "click").subscribe(() => {
            if (!this.subscription2 || this.subscription2.closed) {
                this.subscription2 = licznik$.subscribe(liczba => {
                    input2.value = liczba.toString();
                });
            } else {
                this.subscription2.unsubscribe();
            }
        });
    }
}

const observer = new ClickObserver();
observer.start();
*/
/* Zadanie 11 - Obserwacje:

bez shareReplay:

Każda subskrypcja tworzy osobny strumień obserwujący kliknięcia.
Operator scan przechowuje własny akumulator dla każdej subskrypcji.

Przez to:
- Oba pola input mogą przyjąć różne wartości.
- Jeśli wlączymy jedno pole, poklikamy, nastepnie wyłączymy i włączymy drugie i poklikamy i na końcu znowu włączymy pierwsze to wartość w piewszym oknie się zresetuje.

Z użyciem operatora shareReplay():

Operator shareReplay powoduje współdzielenie jednego strumienia
przez wiele subskrypcji oraz zapamiętuje ostatnią wyemitowaną wartość.

Przez to:
- Oba pula input zawsze będą miały te samą wartość.
- Uodparnia nas to na błąd z przełączaniem.
- Po ponownym kliknięciu otrzymujemy ostatnią zapisaną zawartość licznika

Wniosek:
shareReplay pozwala współdzielić wyniki jednego Observable
i przekazywać nowym subskrybentom ostatnio wyemitowaną wartość.
*/


// zadanie 12
/*
const input = document.querySelector<HTMLInputElement>("#tekst")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

fromEvent(input, "input")
    .pipe(
        map(() => input.value)
    )
    .subscribe(tekst => {
        paragraf.textContent = tekst;
    });
*/

// zadanie 13
/*
const input = document.querySelector<HTMLInputElement>("#tekst")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

fromEvent(input, "input")
    .pipe(
        map(() => input.value),
        filter(tekst => tekst.length > 5)
    )
    .subscribe(tekst => {
        paragraf.textContent = tekst;
    });
*/
// zadanie 14
/*
const inputTekst = document.querySelector<HTMLInputElement>("#tekst")!;
const inputLiczba = document.querySelector<HTMLInputElement>("#liczba")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

const tekst$ = fromEvent(inputTekst, "input").pipe(
    map(() => inputTekst.value),
    startWith("")
);

const liczba$ = fromEvent(inputLiczba, "input").pipe(
    map(() => inputLiczba.value),
    startWith("")
);

combineLatest([tekst$, liczba$])
    .subscribe(([tekst, liczba]) => {
        const wartosc = Number(liczba);

        if (liczba.trim() === "" || isNaN(wartosc)) {
            paragraf.textContent = tekst;
        } else if (tekst.length > wartosc) {
            paragraf.textContent = tekst;
        } else {
            paragraf.textContent = "";
        }
    });
*/

// Zadanie 15

/*
const inputA = document.querySelector<HTMLInputElement>("#bokA")!;
const inputB = document.querySelector<HTMLInputElement>("#bokB")!;
const button = document.querySelector<HTMLButtonElement>("#oblicz")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

const bokA$ = fromEvent(inputA, "input").pipe(
    map(() => inputA.value),
    startWith("")
);

const bokB$ = fromEvent(inputB, "input").pipe(
    map(() => inputB.value),
    startWith("")
);

fromEvent(button, "click")
    .pipe(
        combineLatest([bokA$, bokB$]),
        map(([, a, b]) => {
            if (a.trim() === "" || b.trim() === "") {
                throw new Error("Oba pola muszą być wypełnione!");
            }

            const liczbaA = Number(a);
            const liczbaB = Number(b);

            if (!Number.isFinite(liczbaA) || !Number.isFinite(liczbaB)) {
                throw new Error("Wprowadź poprawne liczby!");
            }

            return liczbaA * liczbaB;
        })
    )
    .subscribe({
        next: wynik => {
            paragraf.textContent = `Pole prostokąta: ${wynik}`;
        },
        error: blad => {
            paragraf.textContent = blad.message;
        }
    });
*/

// bez startWith funkcja nie zwroci nam ze nie wpisano zadnych znakow ale z startwith przy wrzuceniu bledow z throw new error aplikacja juz nie wczyta nowych wartosci bo pokazala error


//Zadanie 16

/*
const inputA = document.querySelector<HTMLInputElement>("#bokA")!;
const inputB = document.querySelector<HTMLInputElement>("#bokB")!;
const button = document.querySelector<HTMLButtonElement>("#oblicz")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

let liczbaBledow = 0;

fromEvent(button, "click")
    .pipe(
        mergeMap(() => {
            return of([inputA.value, inputB.value]).pipe(
                mergeMap(([a, b]) => {
                    if (
                        a.trim() === "" ||
                        b.trim() === "" ||
                        !Number.isFinite(Number(a)) ||
                        !Number.isFinite(Number(b))
                    ) {
                        return throwError(() =>
                            new Error("Wprowadzono błędną wartość!")
                        );
                    }

                    return of(Number(a) * Number(b));
                }),

                catchError(blad => {
                    liczbaBledow++;

                    if (liczbaBledow >= 4) {
                        paragraf.textContent =
                            "Wprowadziłeś błędną wartość 4 razy. Koniec";

                        button.disabled = true;
                    } else {
                        paragraf.textContent =
                            blad.message +
                            ` Pozostało prób: ${4 - liczbaBledow}`;
                    }

                    return of(null);
                })
            );
        })
    )
    .subscribe(wynik => {
        if (wynik !== null) {
            paragraf.textContent = `Pole prostokąta: ${wynik}`;
        }
    });
*/
// Zadanie 17

/*
const inputA = document.querySelector<HTMLInputElement>("#bokA")!;
const inputB = document.querySelector<HTMLInputElement>("#bokB")!;
const button = document.querySelector<HTMLButtonElement>("#oblicz")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

const bokA$ = fromEvent(inputA, "input").pipe(
    map(() => inputA.value),
    startWith(inputA.value)
);

const bokB$ = fromEvent(inputB, "input").pipe(
    map(() => inputB.value),
    startWith(inputB.value)
);

fromEvent(button, "click")
    .pipe(
        withLatestFrom(bokA$, bokB$),

        mergeMap(([, a, b]) => {
            return of([a, b]).pipe(
                mergeMap(([a, b]) => {
                    if (a.trim() === "" || b.trim() === "") {
                        return throwError(() =>
                            new Error("Oba pola muszą być wypełnione!")
                        );
                    }

                    const liczbaA = Number(a);
                    const liczbaB = Number(b);

                    if (
                        !Number.isFinite(liczbaA) ||
                        !Number.isFinite(liczbaB)
                    ) {
                        return throwError(() =>
                            new Error("Wprowadź poprawne liczby!")
                        );
                    }

                    return of(liczbaA * liczbaB);
                }),

                catchError(blad => {
                    paragraf.textContent = blad.message;
                    return of(null);
                })
            );
        })
    )
    .subscribe(wynik => {
        if (wynik !== null) {
            paragraf.textContent = `Pole prostokąta: ${wynik}`;
        }
    });
*/

// zadanie 18
/*
const inputA = document.querySelector<HTMLInputElement>("#bokA")!;
const inputB = document.querySelector<HTMLInputElement>("#bokB")!;
const button = document.querySelector<HTMLButtonElement>("#oblicz")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

const bokA$ = fromEvent(inputA, "input").pipe(
    map(() => inputA.value),
    startWith(inputA.value)
);

const bokB$ = fromEvent(inputB, "input").pipe(
    map(() => inputB.value),
    startWith(inputB.value)
);

fromEvent(button, "click")
    .pipe(
        withLatestFrom(bokA$, bokB$),

        filter(([, a, b]) => a.trim() !== "" && b.trim() !== ""),

        mergeMap(([, a, b]) => {
            return of([a, b]).pipe(
                map(([a, b]) => {
                    const liczbaA = Number(a);
                    const liczbaB = Number(b);

                    if (
                        !Number.isFinite(liczbaA) ||
                        !Number.isFinite(liczbaB)
                    ) {
                        throw new Error("Wprowadź poprawne liczby!");
                    }

                    return liczbaA * liczbaB;
                }),

                catchError(blad => {
                    paragraf.textContent = blad.message;
                    return of(null);
                })
            );
        })
    )
    .subscribe(wynik => {
        if (wynik !== null) {
            paragraf.textContent = `Pole prostokąta: ${wynik}`;
        }
    });

*/

// zadanie 19
/*
const div = document.querySelector<HTMLDivElement>("#kwadrat")!;

fromEvent<MouseEvent>(document, "mousemove")
    .pipe(
        map(event => ({
            x: event.clientX,
            y: event.clientY
        }))
    )
    .subscribe(pozycja => {
        div.style.left = pozycja.x + "px";
        div.style.top = pozycja.y + "px";
    });
*/

// zadanie 20

const div = document.querySelector<HTMLDivElement>("#kwadrat")!;

fromEvent<MouseEvent>(document, "mousemove")
    .pipe(
        map(event => ({
            x: event.clientX,
            y: event.clientY
        })),
        delay(100)
    )
    .subscribe(pozycja => {
        div.style.left = pozycja.x + "px";
        div.style.top = pozycja.y + "px";
    });