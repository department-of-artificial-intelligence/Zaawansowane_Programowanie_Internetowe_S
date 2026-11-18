// Zadanie 15 - pole na podstawie dwóch pól input, błąd obsłużony w subscribe
import { fromEvent, merge } from "rxjs";
import { map } from "rxjs/operators";
import { parseNumber } from "./utils";

export class Zad15 {
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
      .pipe(map(_ => this.computeArea()))
      .subscribe({
        next: area => this.paragraph.textContent = `Pole: ${area}`,
        error: (err: Error) => this.paragraph.textContent = `Błąd: ${err.message}`
      });
  }

  private computeArea(): number {
    return parseNumber(this.inputA.value, "a") * parseNumber(this.inputB.value, "b");
  }
}