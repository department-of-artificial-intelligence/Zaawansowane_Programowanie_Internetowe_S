export class Autor {
    private static nastepneId: number = 1;

    private _id: number = Autor.nastepneId++;
    private _imie!: string;
    private _nazwisko!: string;
    private _email!: string;

    constructor(imie: string, nazwisko: string, email: string) {
        this.imie = imie;
        this.nazwisko = nazwisko;
        this.email = email;
    }

    get imie(): string { return this._imie; }
    set imie(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Imię jest wymagane");
        this._imie = value;
    }

    get nazwisko(): string { return this._nazwisko; }
    set nazwisko(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Nazwisko jest wymagane");
        this._nazwisko = value;
    }

    get email(): string { return this._email; }
    set email(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Email jest wymagany");
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value))
            throw new Error("Niepoprawny adres email");
        this._email = value;
    }
}
