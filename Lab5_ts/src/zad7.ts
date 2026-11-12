// Zadanie 7 - liczba kliknięć przechowywana w polu klasy (operator tap)
import { fromEvent } from "rxjs";
import { tap } from "rxjs/operators";

export class Zad7 {
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;
  private clicks: number = 0;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(tap(() => this.clicks++))
      .subscribe(() => {
        this.paragraph.innerHTML = `Liczba kliknięć: ${this.clicks}`;
      });
  }
}