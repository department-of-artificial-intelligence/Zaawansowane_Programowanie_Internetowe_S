export class Autor {
    private _id: number;
    private _imie: string;
    private _nazwisko: string;
    private _email: string;

    constructor(imie: string, nazwisko: string, email: string) {
        // Walidacja wymaganych pól
        if (!imie?.trim()) throw new Error("Imię jest wymagane.");
        if (!nazwisko?.trim()) throw new Error("Nazwisko jest wymagane.");
        if (!email?.trim()) throw new Error("Email jest wymagany.");

        // Walidacja formatu email (prosty regex — wystarczający dla podstawowej składni)
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            throw new Error("Niepoprawny format adresu email.");
        }

        this._imie = imie.trim();
        this._nazwisko = nazwisko.trim();
        this._email = email.trim();
        this._id = 0; // ID nadal może być ustawiane później
    }

    get imie(): string {
        return this._imie;
    }

    set imie(value: string) {
        if (!value?.trim()) throw new Error("Imię nie może być puste.");
        this._imie = value.trim();
    }

    get nazwisko(): string {
        return this._nazwisko;
    }

    set nazwisko(value: string) {
        if (!value?.trim()) throw new Error("Nazwisko nie może być puste.");
        this._nazwisko = value.trim();
    }

    get email(): string {
        return this._email;
    }

    set email(value: string) {
        if (!value?.trim()) throw new Error("Email jest wymagany.");
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            throw new Error("Niepoprawny format adresu email.");
        }
        this._email = value.trim();
    }
}
