// Zadanie 16 - użytkownik może się poprawić maksymalnie 3 razy
// (catchError + throwError + retry)
import { fromEvent, merge, throwError } from "rxjs";
import { catchError, map, retry } from "rxjs/operators";
import { parseNumber } from "./utils";

export class Zad16 {
  private inputA: HTMLInputElement;
  private inputB: HTMLInputElement;
  private paragraph: HTMLParagraphElement;

  constructor(inputAId: string, inputBId: string, paragraphId: string) {
    this.inputA = document.querySelector<HTMLInputElement>(`#${inputAId}`)!;
    this.inputB = document.querySelector<HTMLInputElement>(`#${inputBId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    merge(fromEvent(this.inputA, "input"), fromEvent(this.inputB, "input"))
      .pipe(
        map(_ => this.computeArea()),
        catchError((err: Error) => {
          this.paragraph.textContent = `Błąd: ${err.message}. Popraw wartość.`;
          return throwError(() => err);
        }),
        retry(3)
      )
      .subscribe({
        next: area => this.paragraph.textContent = `Pole: ${area}`,
        error: () => this.paragraph.textContent =
          "Wprowadziłeś błędną wartość 4 razy. Koniec zabawy"
      });
  }

  private computeArea(): number {
    return parseNumber(this.inputA.value, "a") * parseNumber(this.inputB.value, "b");
  }
}