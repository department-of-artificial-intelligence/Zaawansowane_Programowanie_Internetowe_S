class Autor {
    get imie() {
        return this._imie;
    }
    get nazwisko() {
        return this._nazwisko;
    }
    get email() {
        return this._email;
    }
    constructor(imie, nazwisko, email) {
        if (!imie || !nazwisko || !email) {
            throw "Wszystkie pola są wymagane.";
        }
        if (!this.isValid(email)) {
            throw "Niepoprawny email.";
        }
        this._imie = imie;
        this._nazwisko = nazwisko;
    }
    isValid(email) {
        const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return emailRegex.test(email);
    }
}
