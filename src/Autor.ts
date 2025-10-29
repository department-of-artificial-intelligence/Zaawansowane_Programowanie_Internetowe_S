export class Autor {
    private _id: number;
    private _imie: string;
    private _nazwisko: string;
    private _email: string;

    constructor(imie: string, nazwisko: string, email: string) {
        this._imie = imie;
        this._nazwisko = nazwisko;
        this._email = email;
        this._id = 0;
    }

    get id(): number {
        return this._id;
    }

    set id(value: number) {
        this._id = value;
    }

    get imie(): string {
        return this._imie;
    }

    set imie(value: string) {
        this._imie = value;
    }

    get nazwisko(): string {
        return this._nazwisko;
    }

    set nazwisko(value: string) {
        this._nazwisko = value;
    }

    get email(): string {
        return this._email;
    }

    set email(value: string) {
        this._email = value;
    }
}
