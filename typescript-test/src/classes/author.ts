export class Author {
  private static _idCounter: number = 1;
  private _id: number;
  private _firstName!: string;
  private _lastName!: string;
  private _email!: string;

  constructor(firstName: string, lastName: string, email: string) {
    this._id = Author._idCounter++;
    this.firstName = firstName;
    this.lastName = lastName;
    this.email = email;
  }
  get id(): number {
    return this._id;
  }
  get firstName(): string {
    return this._firstName;
  }
  set firstName(value: string) {
    if (!value || value.trim().length === 0) {
      throw new Error("Imie autora jest wymagane");
    }
    this._firstName = value.trim();
  }
  get lastName(): string {
    return this._lastName;
  }
  set lastName(value: string) {
    if (!value || value.trim().length === 0) {
      throw new Error("Nazwisko autora jest wymagane");
    }
    this._lastName = value.trim();
  }
  get email(): string {
    return this._email;
  }
  set email(value: string) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!value || !emailRegex.test(value.trim())) {
      throw new Error("Nazwisko autora jest wymagane");
    }
    this._email = value.trim();
  }
}
