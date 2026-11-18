// Zadanie 27 - porównanie switchMap, mergeMap i concatMap.
// Każde kliknięcie wysyła żądanie, którego odpowiedź jest dodatkowo opóźniona (delay).
import { fromEvent, Observable, OperatorFunction } from "rxjs";
import { concatMap, delay, map, mergeMap, switchMap, tap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";

type FlatteningOperator = (project: (id: number) => Observable<string>)
  => OperatorFunction<number, string>;

export class Zad27 {
  private button: HTMLButtonElement;
  private logElement: HTMLElement;
  private flatten: FlatteningOperator;
  private requestNumber = 0;

  constructor(buttonId: string, logId: string, flatten: FlatteningOperator) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.logElement = document.querySelector<HTMLElement>(`#${logId}`)!;
    this.flatten = flatten;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(
        map(_ => ++this.requestNumber),
        this.flatten(id => this.request(id))
      )
      .subscribe(text => this.log(text));
  }

  // jedno "żądanie do serwera" opóźnione o 1-3 s
  private request(id: number): Observable<string> {
    const ms = 1000 + Math.floor(Math.random() * 2000);
    return ajax("http://localhost:9000/dane.json").pipe(
      delay(ms),
      map(_ => `#${id} ZAKOŃCZONE (po ${ms} ms)`),
      tap({
        subscribe: () => this.log(`#${id} wysłane`),
        unsubscribe: () => this.log(`#${id} anulowane`)
      })
    );
  }

  private log(text: string): void {
    const time = new Date().toLocaleTimeString();
    this.logElement.innerHTML += `${time} - ${text}<br>`;
  }
}

// trzy warianty różniące się wyłącznie operatorem
export const zad27Variants = {
  switchMap: (project: (id: number) => Observable<string>) => switchMap(project),
  mergeMap: (project: (id: number) => Observable<string>) => mergeMap(project),
  concatMap: (project: (id: number) => Observable<string>) => concatMap(project)
};