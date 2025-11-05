class Autor {
    private _id: number;
    private _imie: string;
    private _nazwisko: string;
    private _email: string;

    get imie() { return this._imie; }
    get nazwisko() { return this._nazwisko; }
    get email() { return this._email; }

    constructor(imie: string, nazwisko: string, email: string) {
        if (!imie || imie.trim() === "") {
            throw new Error("Pole 'imie' jest wymagane.");
        }
        if (!nazwisko || nazwisko.trim() === "") {
            throw new Error("Pole 'nazwisko' jest wymagane.");
        }
        if (!email || email.trim() === "") {
            throw new Error("Pole 'email' jest wymagane.");
        }
        if (!Autor.czyPoprawnyEmail(email)) {
            throw new Error(`Niepoprawny adres e-mail: ${email}`);
        }

        this._imie = imie.trim();
        this._nazwisko = nazwisko.trim();
        this._email = email.trim();
    }

    private static czyPoprawnyEmail(email: string): boolean {
        const wzorzecEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return wzorzecEmail.test(email);
    }
}