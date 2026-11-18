// Zadanie 23 - formularz wysyłany metodą POST w formacie JSON na adres /proceed
import { fromEvent } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";

type Person = {
  firstName: string;
  lastName: string;
  email: string;
};

export class Zad23 {
  private form: HTMLFormElement;
  private firstName: HTMLInputElement;
  private lastName: HTMLInputElement;
  private email: HTMLInputElement;
  private result: HTMLElement;

  constructor(formId: string, firstNameId: string, lastNameId: string,
              emailId: string, resultId: string) {
    this.form = document.querySelector<HTMLFormElement>(`#${formId}`)!;
    this.firstName = document.querySelector<HTMLInputElement>(`#${firstNameId}`)!;
    this.lastName = document.querySelector<HTMLInputElement>(`#${lastNameId}`)!;
    this.email = document.querySelector<HTMLInputElement>(`#${emailId}`)!;
    this.result = document.querySelector<HTMLElement>(`#${resultId}`)!;
  }

  public start(): void {
    fromEvent(this.form, "submit")
      .pipe(
        // blokujemy standardowe wysłanie formularza (przeładowanie strony)
        tap(e => e.preventDefault()),
        map(_ => this.readForm()),
        switchMap(person => ajax({
          url: "http://localhost:9000/proceed",
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: person
        }))
      )
      .subscribe({
        next: response => {
          console.log("Zad 23:", response.response);
          this.result.textContent = `Odpowiedź serwera: ${JSON.stringify(response.response)}`;
        },
        error: err => {
          console.error("Zad 23:", err);
          this.result.textContent = `Błąd: ${err.message}`;
        }
      });
  }

  private readForm(): Person {
    return {
      firstName: this.firstName.value,
      lastName: this.lastName.value,
      email: this.email.value
    };
  }
}