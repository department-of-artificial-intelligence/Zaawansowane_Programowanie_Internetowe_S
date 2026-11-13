import { fromEvent, Subscription, Observable, combineLatest, throwError } from "rxjs";
import { tap, map, scan, take, shareReplay, filter, catchError, retry, delay } from "rxjs/operators";

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

const app7 = new ClickCounterApp("clickBtn", "counterOutput");
app7.init();

//ZADANIE 8
class ClickCounterStatelessApp {
    private button: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;

    constructor(buttonId: string, paragraphId: string) {
        this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        fromEvent(this.button, "click")
            .pipe(
                map(() => 1),
                scan((acc: number, curr: number) => acc + curr, 0)
            )
            .subscribe((count: number) => {
                this.paragraph.textContent = `Liczba kliknięć: ${count}`;
            });
    }
}

const app8 = new ClickCounterStatelessApp("clickBtn8", "counterOutput8");
app8.init();

// ZADANIE 9
class ClickCounterLimitedApp {
    private button: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;
    private maxClicks: number;

    constructor(buttonId: string, paragraphId: string, maxClicks: number) {
        this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
        this.maxClicks = maxClicks;
    }

    public init(): void {
        fromEvent(this.button, "click")
            .pipe(
                take(this.maxClicks),
                map(() => 1),
                scan((acc: number, curr: number) => acc + curr, 0)
            )
            .subscribe({
                next: (count: number) => {
                    this.paragraph.textContent = `Liczba kliknięć: ${count}`;
                },
                complete: () => {
                    this.paragraph.textContent += ` (Osiągnięto limit ${this.maxClicks} kliknięć)`;
                    this.button.disabled = true; // opcjonalne zablokowanie przycisku w UI
                }
            });
    }
}

const app9 = new ClickCounterLimitedApp("clickBtn9", "counterOutput9", 5);
app9.init();

// ZADANIE 10
class ControlledClickCounterApp {
    private startButton: HTMLButtonElement;
    private stopButton: HTMLButtonElement;
    private countButton: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;
    private maxClicks: number;
    private subscription?: Subscription;

    constructor(
        startButtonId: string,
        stopButtonId: string,
        countButtonId: string,
        paragraphId: string,
        maxClicks: number
    ) {
        this.startButton = document.querySelector<HTMLButtonElement>(`#${startButtonId}`)!;
        this.stopButton = document.querySelector<HTMLButtonElement>(`#${stopButtonId}`)!;
        this.countButton = document.querySelector<HTMLButtonElement>(`#${countButtonId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
        this.maxClicks = maxClicks;
    }

    public init(): void {
        fromEvent(this.startButton, "click").subscribe(() => this.startCounting());
        fromEvent(this.stopButton, "click").subscribe(() => this.stopCounting());
    }

    private startCounting(): void {
        if (!this.subscription) {
            this.paragraph.textContent = "Status: aktywny | Liczba kliknięć: 0";
            this.subscription = fromEvent(this.countButton, "click")
                .pipe(
                    take(this.maxClicks),
                    map(() => 1),
                    scan((acc: number, curr: number) => acc + curr, 0)
                )
                .subscribe({
                    next: (count: number) => {
                        this.paragraph.textContent = `Status: aktywny | Liczba kliknięć: ${count}`;
                    },
                    complete: () => {
                        this.paragraph.textContent += ` (Osiągnięto limit ${this.maxClicks} kliknięć)`;
                        this.subscription = undefined;
                    }
                });
        }
    }

    private stopCounting(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
            this.subscription = undefined;
            this.paragraph.textContent += " [Zatrzymano]";
        }
    }
}

const app10 = new ControlledClickCounterApp("startBtn10", "stopBtn10", "countBtn10", "counterOutput10", 5);
app10.init();

//ZADANIE 11
class SharedClickCounterApp {
    private countButton: HTMLButtonElement;
    private toggleBtn1: HTMLButtonElement;
    private toggleBtn2: HTMLButtonElement;
    private input1: HTMLInputElement;
    private input2: HTMLInputElement;

    private clicks$: Observable<number>;
    private sub1?: Subscription;
    private sub2?: Subscription;

    constructor(
        countBtnId: string,
        toggleBtn1Id: string,
        toggleBtn2Id: string,
        input1Id: string,
        input2Id: string
    ) {
        this.countButton = document.querySelector<HTMLButtonElement>(`#${countBtnId}`)!;
        this.toggleBtn1 = document.querySelector<HTMLButtonElement>(`#${toggleBtn1Id}`)!;
        this.toggleBtn2 = document.querySelector<HTMLButtonElement>(`#${toggleBtn2Id}`)!;
        this.input1 = document.querySelector<HTMLInputElement>(`#${input1Id}`)!;
        this.input2 = document.querySelector<HTMLInputElement>(`#${input2Id}`)!;

        // Wersja z operator shareReplay(1) rozwiązująca problem niezależnych liczników
        this.clicks$ = fromEvent(this.countButton, "click").pipe(
            map(() => 1),
            scan((acc: number, curr: number) => acc + curr, 0),
            shareReplay(1)
        );
    }

    public init(): void {
        // Przełączanie wyświetlania w pierwszym polu input (przycisk 2)
        fromEvent(this.toggleBtn1, "click").subscribe(() => {
            if (!this.sub1) {
                this.sub1 = this.clicks$.subscribe((count: number) => {
                    this.input1.value = `Kliknięcia: ${count}`;
                });
            } else {
                this.sub1.unsubscribe();
                this.sub1 = undefined;
                this.input1.value = "";
            }
        });

        // Przełączanie wyświetlania w drugim polu input (przycisk 3)
        fromEvent(this.toggleBtn2, "click").subscribe(() => {
            if (!this.sub2) {
                this.sub2 = this.clicks$.subscribe((count: number) => {
                    this.input2.value = `Kliknięcia: ${count}`;
                });
            } else {
                this.sub2.unsubscribe();
                this.sub2 = undefined;
                this.input2.value = "";
            }
        });
    }
}

const app11 = new SharedClickCounterApp(
    "countBtn11",
    "toggleInput1Btn11",
    "toggleInput2Btn11",
    "input1_11",
    "input2_11"
);
app11.init();

// ZADANIE 12
class TextMirrorApp {
    private input: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(inputId: string, paragraphId: string) {
        this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        fromEvent(this.input, "input")
            .pipe(
                map(() => this.input.value)
            )
            .subscribe((text: string) => {
                this.paragraph.textContent = text;
            });
    }
}

const app12 = new TextMirrorApp("textInput12", "textOutput12");
app12.init();

// ZADANIE 13
class FilteredTextMirrorApp {
    private input: HTMLInputElement;
    private paragraph: HTMLParagraphElement;
    private minLength: number;

    constructor(inputId: string, paragraphId: string, minLength: number) {
        this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
        this.minLength = minLength;
    }

    public init(): void {
        fromEvent(this.input, "input")
            .pipe(
                map(() => this.input.value),
                filter((text: string) => text.length > this.minLength)
            )
            .subscribe((text: string) => {
                this.paragraph.textContent = text;
            });
    }
}

const app13 = new FilteredTextMirrorApp("textInput13", "textOutput13", 5);
app13.init();

// ZADANIE 14
class DynamicFilteredTextApp {
    private textInput: HTMLInputElement;
    private lengthInput: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(textInputId: string, lengthInputId: string, paragraphId: string) {
        this.textInput = document.querySelector<HTMLInputElement>(`#${textInputId}`)!;
        this.lengthInput = document.querySelector<HTMLInputElement>(`#${lengthInputId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        const text$ = fromEvent(this.textInput, "input").pipe(
            map(() => this.textInput.value)
        );

        const minLength$ = fromEvent(this.lengthInput, "input").pipe(
            map(() => {
                const raw = this.lengthInput.value.trim();
                return raw === "" ? NaN : Number(raw);
            })
        );

        combineLatest([text$, minLength$])
            .pipe(
                filter(([text, minLen]) => Number.isNaN(minLen) || text.length > minLen),
                map(([text]) => text)
            )
            .subscribe((text: string) => {
                this.paragraph.textContent = text;
            });
    }
}

const app14 = new DynamicFilteredTextApp("textInput14", "minLengthInput14", "textOutput14");
app14.init();

// ZADANIE 15
class AreaCalculatorApp {
    private inputA: HTMLInputElement;
    private inputB: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(inputAId: string, inputBId: string, paragraphId: string) {
        this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
        this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        const sideA$ = fromEvent(this.inputA, "input").pipe(
            map(() => this.inputA.value.trim())
        );

        const sideB$ = fromEvent(this.inputB, "input").pipe(
            map(() => this.inputB.value.trim())
        );

        combineLatest([sideA$, sideB$])
            .pipe(
                map(([rawA, rawB]) => {
                    if (rawA === "" || rawB === "") {
                        throw new Error("Oba pola muszą być wypełnione!");
                    }
                    const a = Number(rawA);
                    const b = Number(rawB);
                    if (Number.isNaN(a) || Number.isNaN(b)) {
                        throw new Error("Wprowadzone wartości muszą być liczbami!");
                    }
                    return a * b;
                })
            )
            .subscribe({
                next: (area: number) => {
                    this.paragraph.textContent = `Pole wynosi: ${area}`;
                },
                error: (err: Error) => {
                    this.paragraph.textContent = `Błąd: ${err.message}`;
                }
            });
    }
}

const app15 = new AreaCalculatorApp("sideA15", "sideB15", "areaOutput15");
app15.init();

// ZADANIE 16
class RetryAreaCalculatorApp {
    private inputA: HTMLInputElement;
    private inputB: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(inputAId: string, inputBId: string, paragraphId: string) {
        this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
        this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        const sideA$ = fromEvent(this.inputA, "input").pipe(
            map(() => this.inputA.value.trim())
        );

        const sideB$ = fromEvent(this.inputB, "input").pipe(
            map(() => this.inputB.value.trim())
        );

        combineLatest([sideA$, sideB$])
            .pipe(
                map(([rawA, rawB]) => {
                    if (rawA === "" || rawB === "") {
                        throw new Error("Oba pola muszą być wypełnione!");
                    }
                    const a = Number(rawA);
                    const b = Number(rawB);
                    if (Number.isNaN(a) || Number.isNaN(b)) {
                        throw new Error("Wprowadzone wartości muszą być liczbami!");
                    }
                    return a * b;
                }),
                catchError((err: Error) => {
                    this.paragraph.textContent = `Błąd: ${err.message} (Spróbuj poprawić)`;
                    return throwError(() => err);
                }),
                retry(3)
            )
            .subscribe({
                next: (area: number) => {
                    this.paragraph.textContent = `Pole wynosi: ${area}`;
                },
                error: () => {
                    this.paragraph.textContent = "Wprowadziłeś błędną wartość 4 razy. Synek, nie ma takiego bicia";
                }
            });
    }
}

const app16 = new RetryAreaCalculatorApp("sideA16", "sideB16", "areaOutput16");
app16.init();

// ZADANIE 17
class InfiniteRetryAreaCalculatorApp {
    private inputA: HTMLInputElement;
    private inputB: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(inputAId: string, inputBId: string, paragraphId: string) {
        this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
        this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        const sideA$ = fromEvent(this.inputA, "input").pipe(
            map(() => this.inputA.value.trim())
        );

        const sideB$ = fromEvent(this.inputB, "input").pipe(
            map(() => this.inputB.value.trim())
        );

        const createCalculationStream = (): Observable<string> =>
            combineLatest([sideA$, sideB$]).pipe(
                map(([rawA, rawB]) => {
                    if (rawA === "" || rawB === "") {
                        throw new Error("Oba pola muszą być wypełnione!");
                    }
                    const a = Number(rawA);
                    const b = Number(rawB);
                    if (Number.isNaN(a) || Number.isNaN(b)) {
                        throw new Error("Wprowadzone wartości muszą być liczbami!");
                    }
                    return `Pole wynosi: ${a * b}`;
                }),
                catchError((err: Error) => {
                    this.paragraph.textContent = `Błąd: ${err.message} (Popraw wartość)`;
                    return createCalculationStream();
                })
            );

        createCalculationStream().subscribe((message: string) => {
            this.paragraph.textContent = message;
        });
    }
}

const app17 = new InfiniteRetryAreaCalculatorApp("sideA17", "sideB17", "areaOutput17");
app17.init();

// ZADANIE 18
class FilteredAreaCalculatorApp {
    private inputA: HTMLInputElement;
    private inputB: HTMLInputElement;
    private paragraph: HTMLParagraphElement;

    constructor(inputAId: string, inputBId: string, paragraphId: string) {
        this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
        this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        const sideA$ = fromEvent(this.inputA, "input").pipe(
            map(() => this.inputA.value.trim()),
            filter((val: string) => val !== "")
        );

        const sideB$ = fromEvent(this.inputB, "input").pipe(
            map(() => this.inputB.value.trim()),
            filter((val: string) => val !== "")
        );

        const createCalculationStream = (): Observable<string> =>
            combineLatest([sideA$, sideB$]).pipe(
                filter(([rawA, rawB]) => rawA !== "" && rawB !== ""),
                map(([rawA, rawB]) => {
                    const a = Number(rawA);
                    const b = Number(rawB);
                    if (Number.isNaN(a) || Number.isNaN(b)) {
                        throw new Error("Wprowadzone wartości muszą być liczbami!");
                    }
                    return `Pole wynosi: ${a * b}`;
                }),
                catchError((err: Error) => {
                    this.paragraph.textContent = `Błąd: ${err.message} (Popraw wartość)`;
                    return createCalculationStream();
                })
            );

        createCalculationStream().subscribe((message: string) => {
            this.paragraph.textContent = message;
        });
    }
}

const app18 = new FilteredAreaCalculatorApp("sideA18", "sideB18", "areaOutput18");
app18.init();

// ZADANIE 19
class MouseFollowerApp {
    private follower: HTMLDivElement;

    constructor(divId: string) {
        this.follower = document.querySelector<HTMLDivElement>(`#${divId}`)!;
    }

    public init(): void {
        fromEvent<MouseEvent>(document, "mousemove")
            .pipe(
                map((e: MouseEvent) => ({
                    x: e.clientX,
                    y: e.clientY
                }))
            )
            .subscribe((pos: { x: number; y: number }) => {
                this.follower.style.left = `${pos.x}px`;
                this.follower.style.top = `${pos.y}px`;
            });
    }
}

const app19 = new MouseFollowerApp("followerDiv19");
app19.init();

// ZADANIE 20
class DelayedMouseFollowerApp {
    private follower: HTMLDivElement;
    private delayMs: number;

    constructor(divId: string, delayMs: number) {
        this.follower = document.querySelector<HTMLDivElement>(`#${divId}`)!;
        this.delayMs = delayMs;
    }

    public init(): void {
        fromEvent<MouseEvent>(document, "mousemove")
            .pipe(
                map((e: MouseEvent) => ({
                    x: e.clientX,
                    y: e.clientY
                })),
                delay(this.delayMs)
            )
            .subscribe((pos: { x: number; y: number }) => {
                this.follower.style.left = `${pos.x}px`;
                this.follower.style.top = `${pos.y}px`;
            });
    }
}

const app20 = new DelayedMouseFollowerApp("followerDiv20", 100);
app20.init();