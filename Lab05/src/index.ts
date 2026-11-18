import {
    fromEvent,
    tap,
    map,
    scan,
    take,
    Subscription,
    Observable,
    shareReplay,
    filter,
    combineLatest,
    startWith,
    catchError,
    retry,
    throwError,
    skip,
    delay,
    switchMap,
    debounceTime,
    distinctUntilChanged,
    of,
    mergeMap,
    concatMap,
    OperatorFunction
} from "rxjs";

import { ajax, AjaxError } from "rxjs/ajax";

// ==========================================
// ZADANIE 7: Obiektowo-reaktywny licznik kliknięć (z polem i tap)
// ==========================================
class ClickCounterApp7 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;
  // Pole klasy przechowujące aktualną liczbę kliknięć
  private count: number = 0;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    this.updateView();

    fromEvent(this.button, "click")
      .pipe(
        // Operator tap wykonuje efekt uboczny (inkrementację pola klasy)
        tap(() => {
          this.count++;
        })
      )
      .subscribe(() => {
        this.updateView();
      });
  }

  private updateView(): void {
    this.paragraph.textContent = `Liczba kliknięć: ${this.count}`;
  }
}

// ==========================================
// ZADANIE 8: Bezstanowy licznik kliknięć (map + scan)
// ==========================================
class ClickCounterApp8 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    this.updateView(0);

    fromEvent(this.button, "click")
      .pipe(
        // 1. Przekształcenie zdarzenia kliknięcia na wartość 1
        map((): number => 1),
        // 2. Akumulacja stanu bez dodatkowego pola w klasie
        scan((acc: number, curr: number): number => acc + curr, 0)
      )
      .subscribe((count: number) => {
        this.updateView(count);
      });
  }

  private updateView(count: number): void {
    this.paragraph.textContent = `Liczba kliknięć: ${count}`;
  }
}

// ==========================================
// ZADANIE 9: Licznik ograniczony do 5 kliknięć (operator take)
// ==========================================
class ClickCounterApp9 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    this.updateView(0);

    fromEvent(this.button, "click")
      .pipe(
        // 1. Przepuszcza tylko 5 pierwszych kliknięć, a następnie kończy strumień
        take(5),
        // 2. Zamienia obiekt zdarzenia MouseEvent na liczbę 1
        map((): number => 1),
        // 3. Zlicza kolejne wartości w strumieniu
        scan((acc: number, curr: number): number => acc + curr, 0)
      )
      .subscribe({
        next: (count: number) => {
          this.updateView(count);
        },
        complete: () => {
          // Automatyczne wywołanie po 5. kliknięciu
          this.button.disabled = true;
          this.paragraph.textContent += " (osiągnięto limit 5 kliknięć)";
        }
      });
  }

  private updateView(count: number): void {
    this.paragraph.textContent = `Liczba kliknięć: ${count}`;
  }
}

// ==========================================
// ZADANIE 10: Włączanie i wyłączanie subskrypcji (Start / Stop / Kliknij)
// ==========================================
class ClickCounterApp10 {
  private startButton: HTMLButtonElement;
  private stopButton: HTMLButtonElement;
  private countButton: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;
  // Przechowuje aktywną subskrypcję przycisku 3, umożliwiając jej późniejsze odłączenie
  private subscription?: Subscription;

  constructor(
    startButtonId: string,
    stopButtonId: string,
    countButtonId: string,
    paragraphId: string
  ) {
    this.startButton = document.querySelector<HTMLButtonElement>(`#${startButtonId}`)!;
    this.stopButton = document.querySelector<HTMLButtonElement>(`#${stopButtonId}`)!;
    this.countButton = document.querySelector<HTMLButtonElement>(`#${countButtonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    // Przycisk 1: aktywuje nasłuchiwanie kliknięć przycisku 3
    fromEvent(this.startButton, "click").subscribe(() => {
      this.startCounting();
    });

    // Przycisk 2: odłącza subskrypcję przycisku 3
    fromEvent(this.stopButton, "click").subscribe(() => {
      this.stopCounting();
    });
  }

  private startCounting(): void {
    // Zabezpieczenie przed wielokrotnym uruchomieniem subskrypcji
    if (!this.subscription) {
      this.updateView(0, "aktywny");

      this.subscription = fromEvent(this.countButton, "click")
        .pipe(
          // Zachowanie limitu maksymalnie 5 kliknięć z Zadania 9
          take(5),
          map((): number => 1),
          scan((acc: number, curr: number): number => acc + curr, 0)
        )
        .subscribe({
          next: (count: number) => {
            this.updateView(count, "aktywny");
          },
          complete: () => {
            this.paragraph.textContent += " — osiągnięto limit 5 kliknięć";
            this.subscription = undefined;
          }
        });
    }
  }

  private stopCounting(): void {
    // Odłączenie od obiektu obserwowalnego usuwa nasłuchiwanie na przycisku 3
    if (this.subscription) {
      this.subscription.unsubscribe();
      this.subscription = undefined;
      this.paragraph.textContent += " (zatrzymano nasłuchiwanie)";
    }
  }

  private updateView(count: number, status: string): void {
    this.paragraph.textContent = `Liczba kliknięć: ${count} (status: ${status})`;
  }
}

// ==========================================
// ZADANIE 11: Współdzielenie licznika między wieloma subskrybentami (shareReplay)
// ==========================================
class ClickCounterApp11 {
  private countButton: HTMLButtonElement;
  private toggleBtn1: HTMLButtonElement;
  private toggleBtn2: HTMLButtonElement;
  private input1: HTMLInputElement;
  private input2: HTMLInputElement;

  private sub1?: Subscription;
  private sub2?: Subscription;
  private clicks$: Observable<number>;

  constructor(
    countButtonId: string,
    toggleBtn1Id: string,
    toggleBtn2Id: string,
    input1Id: string,
    input2Id: string
  ) {
    this.countButton = document.querySelector<HTMLButtonElement>(`#${countButtonId}`)!;
    this.toggleBtn1 = document.querySelector<HTMLButtonElement>(`#${toggleBtn1Id}`)!;
    this.toggleBtn2 = document.querySelector<HTMLButtonElement>(`#${toggleBtn2Id}`)!;
    this.input1 = document.querySelector<HTMLInputElement>(`#${input1Id}`)!;
    this.input2 = document.querySelector<HTMLInputElement>(`#${input2Id}`)!;

    // Wspólny strumień zliczający kliknięcia pierwszego przycisku
    this.clicks$ = fromEvent(this.countButton, "click").pipe(
      map((): number => 1),
      scan((acc: number, curr: number): number => acc + curr, 0),
      // Buforuje ostatnią wartość i współdzieli jeden strumień między odbiorcami
      shareReplay(1)
    );
  }

  public init(): void {
    // Obsługa przycisku 2 (przełączanie wyświetlania w Input 1)
    fromEvent(this.toggleBtn1, "click").subscribe(() => {
      if (!this.sub1) {
        this.input1.disabled = false;
        this.input1.placeholder = "Oczekiwanie na kliknięcie...";
        this.sub1 = this.clicks$.subscribe((count: number) => {
          this.input1.value = String(count);
        });
      } else {
        this.sub1.unsubscribe();
        this.sub1 = undefined;
        this.input1.disabled = true;
      }
    });

    // Obsługa przycisku 3 (przełączanie wyświetlania w Input 2)
    fromEvent(this.toggleBtn2, "click").subscribe(() => {
      if (!this.sub2) {
        this.input2.disabled = false;
        this.input2.placeholder = "Oczekiwanie na kliknięcie...";
        this.sub2 = this.clicks$.subscribe((count: number) => {
          this.input2.value = String(count);
        });
      } else {
        this.sub2.unsubscribe();
        this.sub2 = undefined;
        this.input2.disabled = true;
      }
    });
  }
}

// ==========================================
// ZADANIE 12: Reaktywne przepisywanie tekstu z inputa do paragrafu
// ==========================================
class LiveTextApp12 {
  private input: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(inputId: string, paragraphId: string) {
    this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    // Nasłuchiwanie zdarzenia 'input' reaguje zarówno na wpisywanie z klawiatury,
    // jak i na wklejenie lub wycięcie tekstu myszką
    fromEvent(this.input, "input")
      .pipe(
        // Odczyt aktualnej wartości pola tekstowego
        map((): string => this.input.value)
      )
      .subscribe((text: string) => {
        this.paragraph.textContent = text;
      });
  }
}

// ==========================================
// ZADANIE 13: Filtrowanie tekstu po liczbie znaków (operator filter)
// ==========================================
class LiveTextApp13 {
  private input: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(inputId: string, paragraphId: string) {
    this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    fromEvent(this.input, "input")
      .pipe(
        // 1. Odczyt aktualnej wartości pola tekstowego
        map((): string => this.input.value),
        // 2. Zatrzymanie zdarzeń, dopóki długość tekstu nie przekroczy 5 znaków
        filter((text: string): boolean => text.length > 5)
      )
      .subscribe((text: string) => {
        this.paragraph.textContent = text;
      });
  }
}

// ==========================================
// ZADANIE 14: Łączenie strumieni z dwóch pól input (combineLatest)
// ==========================================
class LiveTextApp14 {
  private textInput: HTMLInputElement;
  private numberInput: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(textInputId: string, numberInputId: string, paragraphId: string) {
    this.textInput = document.querySelector<HTMLInputElement>(`#${textInputId}`)!;
    this.numberInput = document.querySelector<HTMLInputElement>(`#${numberInputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    // 1. Strumień aktualnego tekstu z pierwszego pola input
    const text$ = fromEvent(this.textInput, "input").pipe(
      map((): string => this.textInput.value),
      startWith(this.textInput.value)
    );

    // 2. Strumień wartości liczbowej z drugiego pola input (type="number")
    const minLength$ = fromEvent(this.numberInput, "input").pipe(
      map((): number => {
        const raw = this.numberInput.value.trim();
        return raw === "" ? NaN : Number(raw);
      }),
      startWith(NaN)
    );

    // 3. Połączenie obu strumieni – reaguje na zmianę w dowolnym z pól
    combineLatest([text$, minLength$])
      .pipe(
        // Jeżeli wartość w drugim polu nie jest liczbą (NaN), przepuszczamy cały tekst;
        // w przeciwnym razie sprawdzamy, czy długość tekstu przekracza podaną liczbę
        filter(([text, minLength]: [string, number]): boolean => {
          if (Number.isNaN(minLength)) {
            return true;
          }
          return text.length > minLength;
        }),
        map(([text]: [string, number]): string => text)
      )
      .subscribe((text: string) => {
        this.paragraph.textContent = text;
      });
  }
}

// ==========================================
// ZADANIE 15: Obliczanie pola i obsługa błędu w subscribe
// ==========================================
class SquareAreaApp15 {
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
      map((): string => this.inputA.value)
    );

    const sideB$ = fromEvent(this.inputB, "input").pipe(
      map((): string => this.inputB.value)
    );

    // combineLatest wyemituje pierwszą parę wartości, gdy użytkownik wprowadzi dane do obu pól
    combineLatest([sideA$, sideB$])
      .pipe(
        map(([rawA, rawB]: [string, string]): number => {
          const trimmedA = rawA.trim();
          const trimmedB = rawB.trim();
          const numA = Number(trimmedA);
          const numB = Number(trimmedB);

          // Weryfikacja, czy którekolwiek pole jest puste lub nie reprezentuje liczby
          if (
            trimmedA === "" ||
            trimmedB === "" ||
            !Number.isFinite(numA) ||
            !Number.isFinite(numB)
          ) {
            throw new Error("Błąd: oba pola muszą być wypełnione poprawnymi liczbami!");
          }

          return numA * numB;
        })
      )
      .subscribe({
        next: (area: number) => {
          this.paragraph.style.color = "inherit";
          this.paragraph.textContent = `Wynik (pole): ${area}`;
        },
        error: (err: Error) => {
          this.paragraph.style.color = "red";
          this.paragraph.textContent = err.message;
        }
      });
  }
}

// ==========================================
// ZADANIE 16: Maksymalnie 3 próby poprawy (catchError + retry + throwError)
// ==========================================
class SquareAreaApp16 {
  private inputA: HTMLInputElement;
  private inputB: HTMLInputElement;
  private paragraph: HTMLParagraphElement;
  private touchedA: boolean = false;
  private touchedB: boolean = false;

  constructor(inputAId: string, inputBId: string, paragraphId: string) {
    this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
    this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    const sideA$ = fromEvent(this.inputA, "input").pipe(
      tap(() => {
        this.touchedA = true;
      }),
      startWith(null),
      map((): string => this.inputA.value)
    );

    const sideB$ = fromEvent(this.inputB, "input").pipe(
      tap(() => {
        this.touchedB = true;
      }),
      startWith(null),
      map((): string => this.inputB.value)
    );

    combineLatest([sideA$, sideB$])
      .pipe(
        // Pomija pierwszą synchroniczną emisję wynikającą ze startWith przy subskrypcji i ponowieniu
        skip(1),
        // Rozpoczyna walidację dopiero po pierwszym wpisaniu danych w oba pola
        filter((): boolean => this.touchedA && this.touchedB),
        map(([rawA, rawB]: [string, string]): number => {
          const trimmedA = rawA.trim();
          const trimmedB = rawB.trim();
          const numA = Number(trimmedA);
          const numB = Number(trimmedB);

          if (
            trimmedA === "" ||
            trimmedB === "" ||
            !Number.isFinite(numA) ||
            !Number.isFinite(numB)
          ) {
            throw new Error("Błąd: wprowadź poprawne liczby w oba pola!");
          }

          return numA * numB;
        }),
        // 1. Przechwycenie błędu, wyświetlenie komunikatu i ponowne rzucenie błędu dla operatora retry
        catchError((err: Error) => {
          this.paragraph.style.color = "#d97706";
          this.paragraph.textContent = `${err.message} Spróbuj poprawić wartość.`;
          return throwError(() => err);
        }),
        // 2. Ponowne zasubskrybowanie strumienia maksymalnie 3 razy po wystąpieniu błędu
        retry(3)
      )
      .subscribe({
        next: (area: number) => {
          this.paragraph.style.color = "inherit";
          this.paragraph.textContent = `Wynik (pole): ${area}`;
        },
        error: () => {
          // Wywołuje się dopiero po 4. błędnej próbie (gdy retry(3) wyczerpie limit)
          this.paragraph.style.color = "red";
          this.paragraph.textContent = "Wprowadziłeś błędną wartość 4 razy. Koniec zabawy";
          this.inputA.disabled = true;
          this.inputB.disabled = true;
        }
      });
  }
}

// ==========================================
// ZADANIE 17: Nieskończona liczba prób poprawy (catchError)
// ==========================================
class SquareAreaApp17 {
  private inputA: HTMLInputElement;
  private inputB: HTMLInputElement;
  private paragraph: HTMLParagraphElement;
  private touchedA: boolean = false;
  private touchedB: boolean = false;

  constructor(inputAId: string, inputBId: string, paragraphId: string) {
    this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
    this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public init(): void {
    const sideA$ = fromEvent(this.inputA, "input").pipe(
      tap(() => {
        this.touchedA = true;
      }),
      startWith(null),
      map((): string => this.inputA.value)
    );

    const sideB$ = fromEvent(this.inputB, "input").pipe(
      tap(() => {
        this.touchedB = true;
      }),
      startWith(null),
      map((): string => this.inputB.value)
    );

    combineLatest([sideA$, sideB$])
      .pipe(
        // Pomija pierwszą synchroniczną emisję wynikającą ze startWith przy starcie i ponowieniu
        skip(1),
        // Rozpoczyna obliczenia dopiero po pierwszym wprowadzeniu danych w oba pola
        filter((): boolean => this.touchedA && this.touchedB),
        map(([rawA, rawB]: [string, string]): number => {
          const trimmedA = rawA.trim();
          const trimmedB = rawB.trim();
          const numA = Number(trimmedA);
          const numB = Number(trimmedB);

          if (
            trimmedA === "" ||
            trimmedB === "" ||
            !Number.isFinite(numA) ||
            !Number.isFinite(numB)
          ) {
            throw new Error("Błąd: wprowadź poprawne liczby w oba pola!");
          }

          return numA * numB;
        }),
        // Przechwycenie błędu, wyświetlenie komunikatu i ponowne zasubskrybowanie
        // do źródłowego strumienia (caught) bez ograniczania liczby prób
        catchError((err: Error, caught) => {
          this.paragraph.style.color = "red";
          this.paragraph.textContent = `${err.message} Popraw wartość.`;
          return caught;
        })
      )
      .subscribe((area: number) => {
        this.paragraph.style.color = "inherit";
        this.paragraph.textContent = `Wynik (pole): ${area}`;
      });
  }
}

// ==========================================
// ZADANIE 18: Ignorowanie pustych pól za pomocą operatora filter
// ==========================================
class SquareAreaApp18 {
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
      startWith(null),
      map((): string => this.inputA.value)
    );

    const sideB$ = fromEvent(this.inputB, "input").pipe(
      startWith(null),
      map((): string => this.inputB.value)
    );

    combineLatest([sideA$, sideB$])
      .pipe(
        // Pomija pierwszą synchroniczną emisję wynikającą ze startWith przy starcie i ponowieniu
        skip(1),
        // Zatrzymuje przetwarzanie, dopóki którekolwiek z pól jest puste
        filter(([rawA, rawB]: [string, string]): boolean => {
          return rawA.trim() !== "" && rawB.trim() !== "";
        }),
        map(([rawA, rawB]: [string, string]): number => {
          const numA = Number(rawA.trim());
          const numB = Number(rawB.trim());

          // Błąd zgłaszany jest wyłącznie wtedy, gdy wpisana niepusta wartość nie jest liczbą
          if (!Number.isFinite(numA) || !Number.isFinite(numB)) {
            throw new Error("Błąd: wprowadzona wartość nie jest poprawną liczbą!");
          }

          return numA * numB;
        }),
        // Pozwala na nieskończoną liczbę poprawek po wystąpieniu błędu
        catchError((err: Error, caught) => {
          this.paragraph.style.color = "red";
          this.paragraph.textContent = `${err.message} Popraw wartość.`;
          return caught;
        })
      )
      .subscribe((area: number) => {
        this.paragraph.style.color = "inherit";
        this.paragraph.textContent = `Wynik (pole): ${area}`;
      });
  }
}

// ==========================================
// ZADANIE 19: Śledzenie kursora przez element div
// ==========================================
interface CursorPosition {
  x: number;
  y: number;
}

class CursorFollowerApp19 {
  private box: HTMLDivElement;

  constructor(boxId: string) {
    this.box = document.querySelector<HTMLDivElement>(`#${boxId}`)!;
  }

  public init(): void {
    fromEvent<MouseEvent>(document, "mousemove")
      .pipe(
        // Przekształcenie obiektu zdarzenia MouseEvent na współrzędne (x, y)
        map((event: MouseEvent): CursorPosition => ({
          x: event.clientX,
          y: event.clientY
        }))
      )
      .subscribe((pos: CursorPosition) => {
        this.box.style.left = `${pos.x}px`;
        this.box.style.top = `${pos.y}px`;
      });
  }
}

// ==========================================
// ZADANIE 20: Śledzenie kursora z opóźnieniem 100 ms (operator delay)
// ==========================================
class CursorFollowerApp20 {
  private box: HTMLDivElement;

  constructor(boxId: string) {
    this.box = document.querySelector<HTMLDivElement>(`#${boxId}`)!;
  }

  public init(): void {
    fromEvent<MouseEvent>(document, "mousemove")
      .pipe(
        // 1. Odczyt bieżących współrzędnych kursora w momencie wystąpienia zdarzenia
        map((event: MouseEvent): CursorPosition => ({
          x: event.clientX,
          y: event.clientY
        })),
        // 2. Opóźnienie przekazania każdej pozycji do subskrybenta o 100 ms
        delay(100)
      )
      .subscribe((pos: CursorPosition) => {
        this.box.style.left = `${pos.x}px`;
        this.box.style.top = `${pos.y}px`;
      });
  }
}

// ==========================================
// ZADANIE 21: Pobieranie pliku dane.json za pomocą funkcji ajax
// ==========================================
class AjaxDataApp21 {
    private url: string;

    constructor(url: string) {
        this.url = url;
    }

    public init(): void {
        console.log("%c--- ZADANIE 21 ---", "color: #3b82f6; font-weight: bold;");

        ajax(this.url).subscribe({
            next: (res) => {
                console.log("Pełny obiekt odpowiedzi (AjaxResponse):", res);
                console.log("Pobrane dane z dane.json:", res.response);
            },
            error: (err) => {
                console.error("Błąd podczas pobierania pliku dane.json:", err);
            }
        });
    }
}

// ==========================================
// ZADANIE 22: Pobieranie dane.json przez ajax i renderowanie tabeli na stronie
// ==========================================
class AjaxTableApp22 {
    private url: string;
    private container: HTMLDivElement;

    constructor(url: string, containerId: string) {
        this.url = url;
        this.container = document.querySelector<HTMLDivElement>(`#${containerId}`)!;
    }

    public init(): void {
        ajax(this.url)
            .pipe(
                // Wydobycie samej zawartości JSON z obiektu AjaxResponse
                map((res) => res.response)
            )
            .subscribe({
                next: (data: unknown) => {
                    this.renderTable(data);
                },
                error: (err: Error) => {
                    this.container.style.color = "red";
                    this.container.textContent = `Błąd podczas pobierania danych: ${err.message}`;
                }
            });
    }

    private renderTable(data: unknown): void {
        this.container.innerHTML = "";

        const table = document.createElement("table");
        table.style.borderCollapse = "collapse";
        table.style.marginTop = "10px";

        const thead = document.createElement("thead");
        const tbody = document.createElement("tbody");

        // Wariant 1: dane.json zawiera tablicę obiektów (np. listę użytkowników/produktów)
        if (Array.isArray(data) && data.length > 0 && typeof data[0] === "object" && data[0] !== null) {
            const headers = Object.keys(data[0] as Record<string, unknown>);
            const headerRow = document.createElement("tr");

            headers.forEach((key: string) => {
                const th = this.createStyledCell("th", key);
                headerRow.appendChild(th);
            });
            thead.appendChild(headerRow);

            data.forEach((item: unknown) => {
                const row = document.createElement("tr");
                const record = item as Record<string, unknown>;

                headers.forEach((key: string) => {
                    const td = this.createStyledCell("td", this.formatValue(record[key]));
                    row.appendChild(td);
                });
                tbody.appendChild(row);
            });
        }
        // Wariant 2: dane.json zawiera pojedynczy obiekt JSON (pary klucz - wartość)
        else if (typeof data === "object" && data !== null) {
            const headerRow = document.createElement("tr");
            headerRow.appendChild(this.createStyledCell("th", "Klucz"));
            headerRow.appendChild(this.createStyledCell("th", "Wartość"));
            thead.appendChild(headerRow);

            Object.entries(data as Record<string, unknown>).forEach(([key, value]) => {
                const row = document.createElement("tr");
                row.appendChild(this.createStyledCell("td", key));
                row.appendChild(this.createStyledCell("td", this.formatValue(value)));
                tbody.appendChild(row);
            });
        } else {
            this.container.textContent = "Brak danych do wyświetlenia w tabeli.";
            return;
        }

        table.appendChild(thead);
        table.appendChild(tbody);
        this.container.appendChild(table);
    }

    private createStyledCell(tag: "th" | "td", text: string): HTMLTableCellElement {
        const cell = document.createElement(tag);
        cell.textContent = text;
        cell.style.border = "1px solid #cbd5e1";
        cell.style.padding = "8px 12px";
        cell.style.textAlign = "left";
        if (tag === "th") {
            cell.style.backgroundColor = "#f1f5f9";
        }
        return cell;
    }

    private formatValue(value: unknown): string {
        if (Array.isArray(value)) {
            return value.join(", ");
        }
        if (typeof value === "object" && value !== null) {
            return JSON.stringify(value);
        }
        return String(value ?? "");
    }
}

// ==========================================
// ZADANIE 23: Wysyłanie danych z formularza metodą POST w formacie JSON (ajax)
// ==========================================
interface UserFormData {
    imie: string;
    nazwisko: string;
    email: string;
}

class FormAjaxApp23 {
    private form: HTMLFormElement;
    private firstNameInput: HTMLInputElement;
    private lastNameInput: HTMLInputElement;
    private emailInput: HTMLInputElement;
    private url: string;

    constructor(
        formId: string,
        firstNameId: string,
        lastNameId: string,
        emailId: string,
        url: string
    ) {
        this.form = document.querySelector<HTMLFormElement>(`#${formId}`)!;
        this.firstNameInput = document.querySelector<HTMLInputElement>(`#${firstNameId}`)!;
        this.lastNameInput = document.querySelector<HTMLInputElement>(`#${lastNameId}`)!;
        this.emailInput = document.querySelector<HTMLInputElement>(`#${emailId}`)!;
        this.url = url;
    }

    public init(): void {
        fromEvent<SubmitEvent>(this.form, "submit")
            .pipe(
                // Zablokowanie domyślnego przeładowania strony po zatwierdzeniu formularza
                tap((event: SubmitEvent) => {
                    event.preventDefault();
                }),
                // Odczyt wartości z trzech pól input i zbudowanie obiektu do wysłania
                map((): UserFormData => ({
                    imie: this.firstNameInput.value,
                    nazwisko: this.lastNameInput.value,
                    email: this.emailInput.value
                })),
                // Przekazanie obiektu konfiguracyjnego żądania POST do funkcji ajax
                switchMap((formData: UserFormData) =>
                    ajax({
                        url: this.url,
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: formData
                    })
                )
            )
            .subscribe({
                next: (res) => {
                    console.log("%c--- ZADANIE 23 ---", "color: #8b5cf6; font-weight: bold;");
                    console.log("Pełna odpowiedź serwera:", res);
                    console.log("Dane zwrócone przez serwer:", res.response);
                },
                error: (err) => {
                    console.error("Błąd podczas wysyłania danych na serwer:", err);
                }
            });
    }
}

// ==========================================
// ZADANIE 24: Obsługa błędu HTTP 404 w funkcji subscribe (rxjs/ajax)
// ==========================================
class AjaxErrorApp24 {
    private url: string;
    private paragraph: HTMLParagraphElement;

    constructor(url: string, paragraphId: string) {
        this.url = url;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        ajax(this.url).subscribe({
            next: (res) => {
                this.paragraph.style.color = "inherit";
                this.paragraph.textContent = `Otrzymano odpowiedź: ${JSON.stringify(res.response)}`;
            },
            error: (err: AjaxError) => {
                this.paragraph.style.color = "red";

                if (err.status === 404) {
                    this.paragraph.textContent =
                        "Błąd 404: Nie znaleziono żądanego zasobu na serwerze (http://localhost:9000/error).";
                } else {
                    this.paragraph.textContent = `Wystąpił błąd połączenia z serwerem (kod: ${err.status || "brak odpowiedzi"}): ${err.message}`;
                }
            }
        });
    }
}

// ==========================================
// ZADANIE 25: Ponawianie żądania przy błędzie 500 (operator retry)
// ==========================================
class AjaxRetryApp25 {
    private url: string;
    private paragraph: HTMLParagraphElement;

    constructor(url: string, paragraphId: string) {
        this.url = url;
        this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
    }

    public init(): void {
        this.paragraph.textContent = "Łączenie z serwerem...";

        ajax(this.url)
            .pipe(
                // 1 żądanie początkowe + 2 ponowienia = łącznie 3 próby komunikacji z serwerem
                retry(2)
            )
            .subscribe({
                next: (res) => {
                    this.paragraph.style.color = "green";
                    this.paragraph.textContent = `Otrzymano poprawną odpowiedź: ${JSON.stringify(res.response)}`;
                },
                error: (err: AjaxError) => {
                    this.paragraph.style.color = "red";
                    this.paragraph.textContent = `Błąd komunikacji z serwerem po 3 próbach (status: ${err.status || "brak odpowiedzi"}): ${err.message}`;
                }
            });
    }
}

// ==========================================
// ZADANIE 26: Wyszukiwarka z debounceTime, distinctUntilChanged i switchMap
// ==========================================
class SearchAjaxApp26 {
    private input: HTMLInputElement;
    private output: HTMLDivElement;
    private url: string;

    constructor(inputId: string, outputId: string, url: string) {
        this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
        this.output = document.querySelector<HTMLDivElement>(`#${outputId}`)!;
        this.url = url;
    }

    public init(): void {
        fromEvent(this.input, "input")
            .pipe(
                // 1. Odczyt aktualnej wartości pola wyszukiwania
                map((): string => this.input.value.trim()),
                // 2. Odczekanie 500 ms od ostatniego naciśnięcia klawisza
                debounceTime(500),
                // 3. Przepuszczenie wartości tylko wtedy, gdy różni się od poprzedniej
                distinctUntilChanged(),
                // 4. Przełączenie na nowe żądanie HTTP i anulowanie poprzedniego niezakończonego
                switchMap((query: string) => {
                    if (query === "") {
                        return of({ query: "", data: null, error: null });
                    }

                    this.output.style.color = "inherit";
                    this.output.textContent = `Pobieranie danych dla frazy "${query}"...`;

                    return ajax(this.url).pipe(
                        map((res) => ({ query, data: res.response, error: null })),
                        // Obsługa błędu wewnątrz switchMap chroni główny strumień przed przerwaniem
                        catchError((err: AjaxError) =>
                            of({
                                query,
                                data: null,
                                error: `Błąd pobierania danych (status: ${err.status || "brak połączenia"}): ${err.message}`
                            })
                        )
                    );
                })
            )
            .subscribe((result) => {
                if (result.query === "") {
                    this.output.style.color = "inherit";
                    this.output.textContent = "Wpisz tekst, aby wyszukać dane...";
                    return;
                }

                if (result.error) {
                    this.output.style.color = "red";
                    this.output.textContent = result.error;
                    return;
                }

                this.output.style.color = "inherit";
                this.output.innerHTML = "";

                const header = document.createElement("p");
                header.textContent = `Wynik z serwera dla zapytania "${result.query}":`;

                const pre = document.createElement("pre");
                pre.style.backgroundColor = "#f1f5f9";
                pre.style.padding = "10px";
                pre.style.borderRadius = "6px";
                pre.textContent = JSON.stringify(result.data, null, 2);

                this.output.appendChild(header);
                this.output.appendChild(pre);
            });
    }
}

// ==========================================
// ZADANIE 27: Porównanie switchMap, mergeMap i concatMap
// ==========================================
class FlatteningOperatorApp27 {
    private button: HTMLButtonElement;
    private logList: HTMLUListElement;
    private flatteningOperator: OperatorFunction<number, string>;

    constructor(
        buttonId: string,
        logListId: string,
        flatteningOperator: OperatorFunction<number, string>
    ) {
        this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
        this.logList = document.querySelector<HTMLUListElement>(`#${logListId}`)!;
        this.flatteningOperator = flatteningOperator;
    }

    public init(): void {
        fromEvent(this.button, "click")
            .pipe(
                // Zliczanie kolejnych kliknięć przycisku
                map((): number => 1),
                scan((acc: number, curr: number): number => acc + curr, 0),
                // Rejestracja momentu kliknięcia w strumieniu wejściowym
                tap((requestId: number) => {
                    this.appendLog(`Kliknięcie #${requestId} (przekazano do operatora)`);
                }),
                // Zastosowanie przekazanego operatora spłaszczającego (switchMap / mergeMap / concatMap)
                this.flatteningOperator
            )
            .subscribe((response: string) => {
                this.appendLog(`Wynik: ${response}`, true);
            });
    }

    private appendLog(message: string, isResult: boolean = false): void {
        const li = document.createElement("li");
        li.textContent = message;
        if (isResult) {
            li.style.fontWeight = "bold";
            li.style.color = "#15803d";
        }
        this.logList.appendChild(li);
    }
}

// Symulacja żądania HTTP z losowym opóźnieniem (800–2500 ms),
// pozwalająca wyraźnie zaobserwować kolejność odpowiedzi
const simulateServerRequest = (requestId: number): Observable<string> => {
    const latency = Math.floor(Math.random() * 1700) + 800;
    return of(`Odpowiedź dla żądania #${requestId} (czas: ${latency} ms)`).pipe(
        delay(latency)
    );
};

// Uruchomienie trzech wariantów różniących się wyłącznie operatorem
const app27Switch = new FlatteningOperatorApp27(
    "btn-switch-27",
    "log-switch-27",
    switchMap(simulateServerRequest)
);
app27Switch.init();

const app27Merge = new FlatteningOperatorApp27(
    "btn-merge-27",
    "log-merge-27",
    mergeMap(simulateServerRequest)
);
app27Merge.init();

const app27Concat = new FlatteningOperatorApp27(
    "btn-concat-27",
    "log-concat-27",
    concatMap(simulateServerRequest)
);
app27Concat.init();

// Uruchomienie wszystkich zadań
const app7 = new ClickCounterApp7("click-btn-7", "counter-output-7");
app7.init();

const app8 = new ClickCounterApp8("click-btn-8", "counter-output-8");
app8.init();

const app9 = new ClickCounterApp9("click-btn-9", "counter-output-9");
app9.init();

// Uruchomienie Zadania 10
const app10 = new ClickCounterApp10(
  "start-btn-10",
  "stop-btn-10",
  "click-btn-10",
  "counter-output-10"
);
app10.init();

// Uruchomienie Zadania 11
const app11 = new ClickCounterApp11(
  "click-btn-11",
  "toggle-btn1-11",
  "toggle-btn2-11",
  "input1-11",
  "input2-11"
);
app11.init();

// Uruchomienie Zadania 12
const app12 = new LiveTextApp12("text-input-12", "text-output-12");
app12.init();

// Uruchomienie Zadania 13
const app13 = new LiveTextApp13("text-input-13", "text-output-13");
app13.init();

// Uruchomienie Zadania 14
const app14 = new LiveTextApp14("text-input-14", "number-input-14", "text-output-14");
app14.init();

// Uruchomienie Zadania 15
const app15 = new SquareAreaApp15("side-a-15", "side-b-15", "area-output-15");
app15.init();

// Uruchomienie Zadania 16
const app16 = new SquareAreaApp16("side-a-16", "side-b-16", "area-output-16");
app16.init();

// Uruchomienie Zadania 17
const app17 = new SquareAreaApp17("side-a-17", "side-b-17", "area-output-17");
app17.init();

// Uruchomienie Zadania 18
const app18 = new SquareAreaApp18("side-a-18", "side-b-18", "area-output-18");
app18.init();

// Uruchomienie Zadania 19
const app19 = new CursorFollowerApp19("follower-box-19");
app19.init();

// Uruchomienie Zadania 20
const app20 = new CursorFollowerApp20("follower-box-20");
app20.init();

// Uruchomienie Zadania 21
const app21 = new AjaxDataApp21("dane.json");
app21.init();

// Uruchomienie Zadania 22
const app22 = new AjaxTableApp22("dane.json", "table-container-22");
app22.init();

// Uruchomienie Zadania 23
const app23 = new FormAjaxApp23(
    "user-form-23",
    "first-name-23",
    "last-name-23",
    "email-23",
    "http://localhost:9000/proceed"
);
app23.init();

// Uruchomienie Zadania 24
const app24 = new AjaxErrorApp24("http://localhost:9000/error", "error-output-24");
app24.init();

// Uruchomienie Zadania 25
const app25 = new AjaxRetryApp25("http://localhost:9000/error1", "retry-output-25");
app25.init();

// Uruchomienie Zadania 26
const app26 = new SearchAjaxApp26(
    "search-input-26",
    "search-output-26",
    "http://localhost:9000/dane.json"
);
app26.init();