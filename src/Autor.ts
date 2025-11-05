export class Autor {
  private _id: number;
  private _imie: string;
  private _nazwisko: string;
  private _email: string;

  constructor(imie: string, nazwisko: string, email: string) {
    if (!imie || !nazwisko || !email) {
      throw new Error("Imię nazwisko i email są wymagane.");
    }

    if (!this.czyEmailPoprawny(email)) {
      throw new Error("Podano niepoprawny adres email.");
    }

    this._id = Date.now();
    this._imie = imie;
    this._nazwisko = nazwisko;
    this._email = email;
  }

  private czyEmailPoprawny(email: string): boolean {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
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

  set email(value: string) {
    if (!this.czyEmailPoprawny(value)) {
      throw new Error("Podano niepoprawny adres email.");
    }
    this._email = value;
  }
}
