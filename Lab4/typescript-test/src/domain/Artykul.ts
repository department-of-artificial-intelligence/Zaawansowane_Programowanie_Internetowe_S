import { Komentarz } from "./Komentarz";

export class Artykul {
    private static nextId = 1;
    private _id: number;
    private _tytul!: string;
    private _tresc: string;
    private _dataUtworzenia: Date;
    private _komentarze: Komentarz[] = [];

    constructor(tytul: string, tresc: string) {
        this._id = Artykul.nextId++;
        this.tytul = tytul;
        this._tresc = tresc ?? "";
        this._dataUtworzenia = new Date(); // Ustalenie daty na aktualną w momencie tworzenia
    }

    get id(): number {
        return this._id;
    }

    get tytul(): string {
        return this._tytul;
    }

    set tytul(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Tytuł artykułu nie może być pusty.");
        }
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

    dodajKomentarz(komentarz: Komentarz): void {
        if (!komentarz) {
            throw new Error("Komentarz musi być poprawnym obiektem.");
        }
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[] {
        return this._komentarze;
    }
}