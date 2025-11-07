export class Autor {
  private _id: number;
  private _imie: string;
  private _nazwisko: string;
  private _email: string;

  constructor(imie: string, nazwisko: string, email: string) {
    this._id = Date.now();
    this._imie = imie;
    this._nazwisko = nazwisko;
    this._email = email;
  }

  get imie(): string {
    return this._imie;
  }

  get nazwisko(): string {
    return this._nazwisko;
  }

  get email(): string {
    return this._email;
  }
}
