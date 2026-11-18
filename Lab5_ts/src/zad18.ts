// Zadanie 18 - brak reakcji, gdy któreś pole jest puste;
// błąd tylko wtedy, gdy wartość nie jest liczbą (filter)
import { fromEvent, merge } from "rxjs";
import { catchError, filter, map } from "rxjs/operators";
import { parseNumber } from "./utils";

export class Zad18 {
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
        filter(_ => this.inputA.value.trim() !== "" && this.inputB.value.trim() !== ""),
        map(_ => this.computeArea()),
        catchError((err: Error, caught) => {
          this.paragraph.textContent = `Błąd: ${err.message}. Popraw wartość.`;
          return caught;
        })
      )
      .subscribe(area => this.paragraph.textContent = `Pole: ${area}`);
  }

  private computeArea(): number {
    return parseNumber(this.inputA.value, "a") * parseNumber(this.inputB.value, "b");
  }
}