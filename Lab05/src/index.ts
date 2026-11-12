import { fromEvent, tap, map, scan } from "rxjs";

// ==========================================
// ZADANIE 7: Obiektowo-reaktywny licznik kliknięć
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
        // bez modyfikowania obiektu zdarzenia płynącego w strumieniu
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
    // Wyświetlenie stanu początkowego przed pierwszym kliknięciem
    this.updateView(0);

    fromEvent(this.button, "click")
      .pipe(
        // 1. Przekształcenie każdego obiektu zdarzenia MouseEvent na liczbę 1
        map((): number => 1),
        // 2. Akumulacja wartości w strumieniu z wartością początkową 0
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

// Uruchomienie obu aplikacji
const app7 = new ClickCounterApp7("click-btn-7", "counter-output-7");
app7.init();

const app8 = new ClickCounterApp8("click-btn-8", "counter-output-8");
app8.init();