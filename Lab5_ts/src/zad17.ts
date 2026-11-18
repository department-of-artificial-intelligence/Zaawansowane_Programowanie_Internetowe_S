// Zadanie 17 - poprawianie wartości w nieskończoność (tylko catchError)
import { fromEvent, merge } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { parseNumber } from "./utils";

export class Zad17 {
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
        // drugi parametr (caught) to strumień źródłowy - zwracając go,
        // subskrybujemy go ponownie, więc strumień nigdy się nie kończy
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