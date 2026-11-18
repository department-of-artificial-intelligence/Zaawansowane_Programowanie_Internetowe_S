// Zadanie 24 - obsługa błędu 404 w funkcji subscribe
import { fromEvent } from "rxjs";
import { switchMap } from "rxjs/operators";
import { ajax, AjaxError } from "rxjs/ajax";

export class Zad24 {
  private button: HTMLButtonElement;
  private result: HTMLElement;

  constructor(buttonId: string, resultId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.result = document.querySelector<HTMLElement>(`#${resultId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(switchMap(_ => ajax("http://localhost:9000/error")))
      .subscribe({
        next: response => this.result.textContent = `Odpowiedź: ${JSON.stringify(response.response)}`,
        error: (err: AjaxError) => {
          this.result.textContent = err.status === 404
            ? `Błąd ${err.status}: nie znaleziono zasobu ${err.request.url}`
            : `Błąd ${err.status}: ${err.message}`;
        }
      });
  }
}