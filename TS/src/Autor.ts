class Autor {
  _id: number;
  _imie: string;
  _nazwisko: string;
  _email: string;

  get imie(): string {
    return this._imie;
  }
  get nazwisko(): string {
    return this._nazwisko;
  }
  get email(): string {
    return this._email;
  }

  constructor(imie: string, nazwisko: string, email: string) {
    if (!imie || !nazwisko || !email) {
      throw "Wszystkie pola są wymagane.";
    }
    if (!this.isValid(email)) {
      throw "Niepoprawny email.";
    }
    this._imie = imie;
    this._nazwisko = nazwisko;
  }

  isValid(email: string): boolean {
    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return emailRegex.test(email);
  }
}
