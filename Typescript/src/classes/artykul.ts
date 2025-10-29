import { Komentarz } from "./komentarz";

export class Artykul {
    private _id: number;
    private _tytul: string
    private _tresc: string;
    private _dataUtworzenia: Date;
    private _komentarze: Komentarz[];

    get tytul(): string { return this._tytul }
    set tytul(value: string) { this._tytul = value; }

    get tresc(): string { return this._tresc }
    set tresc(value: string) { this._tresc = value; }

    constructor(tytul: string, tresc: string) {
        this._tresc = tresc;
        this._tytul = tytul;
        this._id = 0;
        this._dataUtworzenia = new Date();
        this._komentarze = [];
    }

    dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[] {
        return this._komentarze;
    }
}