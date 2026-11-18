// Zadanie 14 - tekst wyświetlany, gdy jego długość jest większa niż liczba z drugiego pola.
// Jeśli w drugim polu nie ma liczby - wyświetlamy cały tekst (combineLatest)
import { combineLatest, fromEvent } from "rxjs";
import { map, startWith } from "rxjs/operators";

export class Zad14 {
  private textInput: HTMLInputElement;
  private numberInput: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(textInputId: string, numberInputId: string, paragraphId: string) {
    this.textInput = document.querySelector<HTMLInputElement>(`#${textInputId}`)!;
    this.numberInput = document.querySelector<HTMLInputElement>(`#${numberInputId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    const text$ = fromEvent(this.textInput, "input").pipe(
      map(_ => this.textInput.value),
      startWith(this.textInput.value)
    );
    const minLength$ = fromEvent(this.numberInput, "input").pipe(
      map(_ => parseFloat(this.numberInput.value)),
      startWith(parseFloat(this.numberInput.value))
    );

    combineLatest([text$, minLength$])
      .pipe(map(([text, minLength]) =>
        isNaN(minLength) || text.length > minLength ? text : ""))
      .subscribe(text => this.paragraph.textContent = text);
  }
}