// Zadanie 25 - trzy próby połączenia z /error1, dopiero potem komunikat o błędzie (retry)
import { defer, EMPTY, fromEvent } from "rxjs";
import { catchError, retry, switchMap } from "rxjs/operators";
import { ajax, AjaxError } from "rxjs/ajax";

export class Zad25 {
  private button: HTMLButtonElement;
  private result: HTMLElement;

  constructor(buttonId: string, resultId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.result = document.querySelector<HTMLElement>(`#${resultId}`)!;
  }

  public start(): void {
    // switchMap tworzy osobny strumień dla każdego kliknięcia, dzięki temu
    // błąd kończy tylko to jedno żądanie, a przycisk działa dalej
    fromEvent(this.button, "click")
      .pipe(switchMap(_ => {
        let attempt = 0;
        this.result.innerHTML = "";
        // defer - kod wewnątrz wykonuje się przy każdej (ponownej) subskrypcji,
        // więc widzimy każdą próbę wykonaną przez retry
        return defer(() => {
          attempt++;
          this.log(`Próba ${attempt}...`);
          return ajax<{ message: string }>("http://localhost:9000/error1");
        }).pipe(
          retry(2), // 1 próba + 2 ponowienia = 3 próby
          catchError((err: AjaxError) => {
            this.log(`Nie udało się połączyć po 3 próbach (błąd ${err.status})`);
            return EMPTY;
          })
        );
      }))
      .subscribe(response => this.log(`Sukces: ${response.response.message}`));
  }

  private log(text: string): void {
    this.result.innerHTML += `${text}<br>`;
  }
}