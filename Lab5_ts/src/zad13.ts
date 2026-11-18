// Zadanie 13 - wyświetlamy tylko, gdy tekst ma więcej niż 5 znaków
import { fromEvent } from "rxjs";
import { filter, map } from "rxjs/operators";

export class Zad13 {
  private input: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(inputId: string, paragraphId: string) {
    this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.input, "input")
      .pipe(
        map(_ => this.input.value),
        filter(text => text.length > 5)
      )
      .subscribe(text => this.paragraph.textContent = text);
  }
}