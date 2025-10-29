export class Author {
    private _id: number
    private _imie: string
    private _nazwisko: string
    private _email: string

    constructor(imie: string, nazwisko: string, email: string) {
        const emailRegex = /^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+$/;
        if (!emailRegex.test(email)) {
            throw new Error("Nieprawidłowy format emaila");
        }
        else {
            this._imie = imie
            this._nazwisko = nazwisko
            this._email = email
        }
    }

    get imie() { return this._imie }
    set imie(imie: string) { this._imie = imie }
    get nazwisko() { return this._nazwisko }
    set nazwisko(nazwisko: string) { this._nazwisko = nazwisko }
    get email() { return this._email }
    set email(email: string) {
        const emailRegex = /^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+$/;
        if (!emailRegex.test(email)) {
            throw new Error("Nieprawidłowy format emaila");
        }
    }
}