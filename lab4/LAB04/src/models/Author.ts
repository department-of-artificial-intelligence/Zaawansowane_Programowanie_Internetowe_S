export class Author {
  private _id: number;
  private _firstName: string;
  private _lastName: string;
  private _email: string;

  constructor(id: number, firstName: string, lastName: string, email: string) {
    if (!firstName.trim()) {
      throw new Error("First name cannot be empty");
    }

    if (!lastName.trim()) {
      throw new Error("Last name cannot be empty");
    }

    if (!email.trim()) {
      throw new Error("Email is required");
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(email)) {
      throw new Error("Invalid email address");
    }

    this._id = id;
    this._firstName = firstName;
    this._lastName = lastName;
    this._email = email;

  }

  get firstName(): string {
    return this._firstName;
  }

  set firstName(value: string) {
    this._firstName = value;
  }

  get lastName(): string {
    return this._lastName;
  }

  set lastName(value: string) {
    this._lastName = value;
  }

  get email(): string {
    return this._email;
  }

  set email(value: string) {
    this._email = value;
  }
}