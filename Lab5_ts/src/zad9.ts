// Zadanie 9 - maksymalnie 5 kliknięć (operator take)
import { fromEvent } from "rxjs";
import { map, scan, take } from "rxjs/operators";

export class Zad9 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(
        take(5),
        scan((count, _) => count + 1, 0),
        map(count => `Liczba kliknięć: ${count}`)
      )
      .subscribe({
        next: text => this.paragraph.innerHTML = text,
        complete: () => {
          this.paragraph.innerHTML += " (limit 5 kliknięć osiągnięty)";
          this.button.disabled = true;
        }
      });
  }
}