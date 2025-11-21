export class Komentarz {
    // Pola prywatne
    private _tresc: string;
    private _data: Date;
    private _odpowiedzi: Komentarz[] = [];
    private _nick: string;

    // Konstruktor
    constructor(tresc: string, nick: string) {
        this._tresc = tresc;
        this._nick = nick;
        this._data = new Date(); // Automatyczne ustawienie daty utworzenia
    }

    // Właściwości (Gettery / Settery)
    public get tresc(): string {
        return this._tresc;
    }
    public set tresc(value: string) {
        this._tresc = value;
    }

    public get data(): Date {
        return this._data;
    }
    // Brak settera dla daty - jest tylko do odczytu

    public get nick(): string {
        return this._nick;
    }
    public set nick(value: string) {
        this._nick = value;
    }

    // Metody publiczne
    public dodajOdpowiedz(odpowiedz: Komentarz): void {
        this._odpowiedzi.push(odpowiedz);
    }

    public pobierzOdpowiedzi(): Komentarz[] {
        return this._odpowiedzi;
    }
}