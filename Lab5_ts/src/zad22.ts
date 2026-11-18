// Zadanie 22 - dane z serwera wyświetlone w formie tabeli na stronie
import { fromEvent } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";
import { createTable } from "./table";

type AuthorDTO = {
  id: number;
  firstName: string;
  lastName: string;
};

export class Zad22 {
  private button: HTMLButtonElement;
  private container: HTMLElement;

  constructor(buttonId: string, containerId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.container = document.querySelector<HTMLElement>(`#${containerId}`)!;
  }

  public start(): void {
    fromEvent(this.button, "click")
      .pipe(
        switchMap(_ => ajax<AuthorDTO[]>("http://localhost:9000/dane.json")),
        map(response => createTable(response.response))
      )
      .subscribe({
        next: table => this.container.replaceChildren(table),
        error: err => this.container.textContent = `Błąd: ${err.message}`
      });
  }
}