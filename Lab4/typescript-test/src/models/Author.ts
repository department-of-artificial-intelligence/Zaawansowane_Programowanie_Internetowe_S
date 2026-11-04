import { validate } from "../validation/validate";
import { required, isEmail } from "../validation/rules";

export class Author {
    private static nextId = 1;

    private _id: number;
    private _firstName!: string;
    private _lastName!: string;
    private _email!: string;

    constructor(firstName: string, lastName: string, email: string) {
        // assigning through setters -> validation runs also in the constructor
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this._id = Author.nextId++; // only after successful validation
    }

    get id(): number { return this._id; }

    @validate(required("Imię jest wymagane"))
    set firstName(value: string) { this._firstName = value; }
    get firstName(): string { return this._firstName; }

    @validate(required("Nazwisko jest wymagane"))
    set lastName(value: string) { this._lastName = value; }
    get lastName(): string { return this._lastName; }

    @validate(required("E-mail jest wymagany"), isEmail())
    set email(value: string) { this._email = value; }
    get email(): string { return this._email; }

    toString(): string {
        return `${this._firstName} ${this._lastName} <${this._email}>`;
    }
}
