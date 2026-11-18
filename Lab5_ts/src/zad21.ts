// Zadanie 21 - pobranie pliku dane.json funkcją ajax i wyświetlenie odpowiedzi w konsoli
import { fromEvent } from "rxjs";
import { switchMap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";

export class Zad21 {
  private button: HTMLButtonElement;

  constructor(buttonId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(switchMap(_ => ajax("http://localhost:9000/dane.json")))
      .subscribe({
        next: response => console.log("Zad 21:", response.response),
        error: err => console.error("Zad 21:", err)
      });
  }
}