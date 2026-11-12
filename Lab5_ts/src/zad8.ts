// Zadanie 8 - bez dodatkowego pola, licznik w operatorze scan
import { fromEvent } from "rxjs";
import { map, scan } from "rxjs/operators";

export class Zad8 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(
        scan((count, _) => count + 1, 0),
        map(count => `Liczba kliknięć: ${count}`)
      )
      .subscribe(text => this.paragraph.innerHTML = text);
  }
}