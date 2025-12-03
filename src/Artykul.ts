import { Komentarz } from './Komentarz';

export class Arttykul {
    private _id: number;
    private _tytul: string;
    private _tresc: string;
    private _dataUtworzenia: Date;
    private _komentarze: Komentarz[];

    constructor(tytul: string, tresc: string) {
        if (!tytul?.trim()) throw new Error("Tytuł artykułu nie może być pusty.");
        this._tytul = tytul.trim();
        this._tresc = tresc ?? ""; // przyjmujemy pustą treść, jeśli nie podano
        this._dataUtworzenia = new Date(); // wymóg: aktualna data
        this._komentarze = []; 
        this._id = 0;
    }

    get tytul(): string {
        return this._tytul;
    }

    set tytul(value: string) {
        if (!value?.trim()) throw new Error("Tytuł artykułu nie może być pusty.");
        this._tytul = value.trim();
    }

    get tresc(): string {
        return this._tresc;
    }

    set tresc(value: string) {
        this._tresc = value ?? "";
    }

    get dataUtworzenia(): Date {
        return this._dataUtworzenia;
    }

    // Metody
    dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[] {
        return [...this._komentarze];
    }
}
