// Zadanie 12 - zawartość pola input wyświetlana w paragrafie
import { fromEvent } from "rxjs";
import { map } from "rxjs/operators";

export class Zad12 {
  private input: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(inputId: string, paragraphId: string) {
    this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.input, "input")
      .pipe(map(_ => this.input.value))
      .subscribe(text => this.paragraph.textContent = text);
  }
}