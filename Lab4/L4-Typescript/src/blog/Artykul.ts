import { Komentarz } from "./Komentarz";

export class Artykul {
    private static nastepneId: number = 1;

    private _id: number = Artykul.nastepneId++;
    private _tytul!: string;
    private _tresc: string;
    private _dataUtworzenia: Date = new Date();
    private _komentarze: Komentarz[] = [];

    constructor(tytul: string, tresc: string) {
        this.tytul = tytul;
        this._tresc = tresc;
    }

    get tytul(): string { return this._tytul; }
    set tytul(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Tytuł artykułu nie może być pusty");
        this._tytul = value;
    }

    get tresc(): string { return this._tresc; }
    set tresc(value: string) { this._tresc = value; }

    dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[] {
        return this._komentarze;
    }
}
