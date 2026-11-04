import { poprawnyEmail, wymagany } from "./walidacja";
import type { NowyAutor } from "./dto";

export class Autor {
  private static nastepneId = 1;

  private _id: number;
  // "!" - wartości ustawiają settery wywołane w konstruktorze (rozdz. 4.1.5)
  private _imie!: string;
  private _nazwisko!: string;
  private _email!: string;

  // id jest opcjonalne: nowy autor dostaje kolejny numer,
  // a autor wczytany z serwera (zadanie 5) zachowuje swój identyfikator
  constructor(imie: string, nazwisko: string, email: string, id?: number) {
    // Zadanie 2: przypisanie przez settery = walidacja przy tworzeniu obiektu
    this.imie = imie;
    this.nazwisko = nazwisko;
    this.email = email;

    this._id = id ?? Autor.nastepneId++;
    if (id !== undefined && id >= Autor.nastepneId) {
      Autor.nastepneId = id + 1;
    }
  }

  get id(): number { return this._id; }

  get imie(): string { return this._imie; }
  set imie(value: string) { this._imie = wymagany(value, "imie"); }

  get nazwisko(): string { return this._nazwisko; }
  set nazwisko(value: string) { this._nazwisko = wymagany(value, "nazwisko"); }

  get email(): string { return this._email; }
  set email(value: string) { this._email = poprawnyEmail(value); }

  get pelneImie(): string { return `${this._imie} ${this._nazwisko}`; }

  // Zadanie 10: częściowa aktualizacja. Najpierw walidujemy wszystkie podane pola,
  // dopiero potem je przypisujemy - błąd w jednym polu nie zmienia żadnego.
  aktualizuj(zmiany: Partial<NowyAutor>): void {
    const imie = zmiany.imie !== undefined ? wymagany(zmiany.imie, "imie") : this._imie;
    const nazwisko = zmiany.nazwisko !== undefined ? wymagany(zmiany.nazwisko, "nazwisko") : this._nazwisko;
    const email = zmiany.email !== undefined ? poprawnyEmail(zmiany.email) : this._email;

    this._imie = imie;
    this._nazwisko = nazwisko;
    this._email = email;
  }

  toString(): string {
    return `${this.pelneImie} <${this._email}>`;
  }
}
