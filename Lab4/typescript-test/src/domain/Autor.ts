export class Autor {
    private static nextId = 1;
    private _id: number;
    private _imie!: string;
    private _nazwisko!: string;
    private _email!: string;

    constructor(imie: string, nazwisko: string, email: string) {
        this._id = Autor.nextId++;
        this.imie = imie;
        this.nazwisko = nazwisko;
        this.email = email;
    }

    get id(): number {
        return this._id;
    }

    get imie(): string {
        return this._imie;
    }

    set imie(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Pole 'imie' jest wymagane i nie może być puste.");
        }
        this._imie = value.trim();
    }

    get nazwisko(): string {
        return this._nazwisko;
    }

    set nazwisko(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Pole 'nazwisko' jest wymagane i nie może być puste.");
        }
        this._nazwisko = value.trim();
    }

    get email(): string {
        return this._email;
    }

    set email(value: string) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!value || value.trim().length === 0) {
            throw new Error("Pole 'email' jest wymagane.");
        }
        if (!emailRegex.test(value.trim())) {
            throw new Error("Niepoprawny format adresu e-mail.");
        }
        this._email = value.trim();
    }

    public aktualizuj(zmiany: Partial<Omit<Autor, "id">>): void {
        if (zmiany.imie !== undefined) {
            this.imie = zmiany.imie;
        }
        if (zmiany.nazwisko !== undefined) {
            this.nazwisko = zmiany.nazwisko;
        }
        if (zmiany.email !== undefined) {
            this.email = zmiany.email;
        }
    }

    get pelneImie(): string {
        return `${this._imie} ${this._nazwisko}`;
    }
}