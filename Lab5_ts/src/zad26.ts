// Zadanie 26 - wyszukiwarka: debounceTime + distinctUntilChanged + switchMap
import { fromEvent } from "rxjs";
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";
import { createTable } from "./table";

type AuthorDTO = {
  id: number;
  firstName: string;
  lastName: string;
};

export class Zad26 {
  private input: HTMLInputElement;
  private slowCheckbox: HTMLInputElement;
  private status: HTMLElement;
  private container: HTMLElement;

  constructor(inputId: string, slowCheckboxId: string, statusId: string, containerId: string) {
    this.input = document.querySelector<HTMLInputElement>(`#${inputId}`)!;
    this.slowCheckbox = document.querySelector<HTMLInputElement>(`#${slowCheckboxId}`)!;
    this.status = document.querySelector<HTMLElement>(`#${statusId}`)!;
    this.container = document.querySelector<HTMLElement>(`#${containerId}`)!;
  }

  public start(): void {
    fromEvent(this.input, "input")
      .pipe(
        map(_ => this.input.value.trim()),
        debounceTime(500),
        distinctUntilChanged(),
        tap(text => this.status.textContent = `Wyszukuję: "${text}"...`),
        switchMap(text => ajax<AuthorDTO[]>(this.buildUrl()).pipe(
          map(response => ({ text, authors: this.filterAuthors(response.response, text) }))
        ))
      )
      .subscribe({
        next: ({ text, authors }) => {
          this.status.textContent = `Wynik dla: "${text}" (${authors.length})`;
          this.container.replaceChildren(createTable(authors));
        },
        error: err => this.status.textContent = `Błąd: ${err.message}`
      });
  }

  // przy zaznaczonym checkboksie serwer odpowiada z losowym opóźnieniem 0-3 s,
  // więc starsze żądanie może wrócić później niż nowsze
  private buildUrl(): string {
    const url = "http://localhost:9000/dane.json";
    return this.slowCheckbox.checked
      ? `${url}?delay=${Math.floor(Math.random() * 3000)}`
      : url;
  }

  private filterAuthors(authors: AuthorDTO[], text: string): AuthorDTO[] {
    const phrase = text.toLowerCase();
    return authors.filter(a =>
      `${a.firstName} ${a.lastName}`.toLowerCase().includes(phrase));
  }
}