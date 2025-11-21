export class Autor {
    private _id: number;
    private _imie: string;
    private _nazwisko: string;
    private _email: string;

    // Prosta walidacja regex dla emaila
    private static emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // Konstruktor teraz wymaga wszystkich 3 pól
    constructor(imie: string, nazwisko: string, email: string) {
        this._id = Date.now() + Math.random();

        // Używamy setterów, aby od razu zwalidować dane
        this.imie = imie;
        this.nazwisko = nazwisko;
        this.email = email;
    }

    // --- Właściwości (Gettery / Settery) z walidacją ---

    public get imie(): string {
        return this._imie;
    }
    public set imie(value: string) {
        if (!value) { // Sprawdzenie, czy nie jest puste, null lub undefined
            throw new Error("Imię autora jest wymagane.");
        }
        this._imie = value;
    }

    public get nazwisko(): string {
        return this._nazwisko;
    }
    public set nazwisko(value: string) {
        if (!value) {
            throw new Error("Nazwisko autora jest wymagane.");
        }
        this._nazwisko = value;
    }

    public get email(): string {
        return this._email;
    }
    public set email(value: string) {
        if (!value) {
            throw new Error("Email autora jest wymagany.");
        }
        if (!Autor.emailRegex.test(value)) {
            throw new Error("Podano niepoprawny format adresu email.");
        }
        this._email = value;
    }
}