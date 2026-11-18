export class Author {
    private static nextId: number = 1;

    private _id: number = Author.nextId++;
    private _firstName!: string;
    private _lastName!: string;
    private _email!: string;

    constructor(firstName: string, lastName: string, email: string) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
    }

    get id(): number { return this._id; }

    get firstName(): string { return this._firstName; }
    set firstName(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("First name is required");
        this._firstName = value;
    }

    get lastName(): string { return this._lastName; }
    set lastName(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Last name is required");
        this._lastName = value;
    }

    get email(): string { return this._email; }
    set email(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Email is required");
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value))
            throw new Error("Email address is not valid");
        this._email = value;
    }
}
