import { Komentarz } from "./Komentarz";

export class Artykul {
    private _id: number;
    private _tytul: string;
    private _tresc: string;
    private _dataUtworzenia: Date; // Ten wymóg był już spełniony
    private _komentarze: Komentarz[] = [];

    constructor(tytul: string, tresc: string) {
        this._id = Date.now() + Math.random();
        this._dataUtworzenia = new Date(); // Wymóg 3: data ustalona na aktualną

        this.tytul = tytul; // Walidacja przez setter
        this._tresc = tresc; // Założenie, że treść może być pusta
    }

    // --- Właściwości (Gettery / Settery) z walidacją ---

    public get tytul(): string {
        return this._tytul;
    }
    public set tytul(value: string) {
        if (!value) {
            throw new Error("Tytuł artykułu jest wymagany.");
        }
        this._tytul = value;
    }

    public get tresc(): string {
        return this._tresc;
    }
    public set tresc(value: string) {
        this._tresc = value;
    }

    // Metody publiczne (bez zmian)
    public dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz);
    }

    public pobierzKomentarze(): Komentarz[] {
        return this._komentarze;
    }
}