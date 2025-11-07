class Autor {
    private _id: number;
    @require("Pole imię nie może być puste") private _imie: string;
    @require("Pole nazwisko nie może być puste") private _nazwisko: string;
    @require("Pole email nie może być puste") private _email: string;

    constructor(imie: string, nazwisko: string) {
        this._imie = imie;
        this._nazwisko = nazwisko
    }
}