export class Autor {
  private static _nextId = 1;
  private static readonly EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  private _id: number;
  private _imie: string;
  private _nazwisko: string;
  private _email: string;

  constructor(imie: string, nazwisko: string, email: string) {
    this._id = Autor._nextId++;
    this._imie = imie;
    this._nazwisko = nazwisko;
    this._email = email;
  }

  public get id(): number {
    return this._id;
  }

  public get imie(): string {
    return this._imie;
  }

  public set imie(value: string) {
    if(!value || value.trim().length === 0) {
      throw new Error("Imie nie moze być puste");
    }
    this._imie = value;
  }

  public get nazwisko(): string {
    return this._nazwisko;
  }

  public set nazwisko(value: string) {
    if(!value || value.trim().length === 0){
      throw new Error("Nazwisko nie może być puste");
    }
    this._nazwisko = value;
  }

  public get pelneImie(): string {
    return `${this._imie} ${this._nazwisko}`;
  }

  public get email(): string {
    return this._email;
  }

  public set email(value: string) {
    if(!value || value.trim().length === 0){
      throw new Error("Email nie może być pusty");
    }
    if(!Autor.EMAIL_REGEX.test(value.trim())){
      throw new Error("Niepoprawna składnia email. ")
    }
    this._email = value;
  }
}