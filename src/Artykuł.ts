import { Autor } from './Autor';
import { Komentarz } from './Komentarz';

export class Arttykul {
    private _id: number;
    private _tytul: string;
    private _tresc: string;
    private _dataUtworzenia: Date;
    private _komentarze: Komentarz[];

    constructor(tytul: string, tresc: string, dataUtworzenia?: Date) {
        this._tytul = tytul;
        this._tresc = tresc;
        this._dataUtworzenia = dataUtworzenia || new Date();
        this._komentarze = [];
        this._id = 0;
    }

    get id(): number {
        return this._id;
    }

    set id(value: number) {
        this._id = value;
    }

    get tytul(): string {
        return this._tytul;
    }

    set tytul(value: string) {
        this._tytul = value;
    }

    get tresc(): string {
        return this._tresc;
    }

    set tresc(value: string) {
        this._tresc = value;
    }

    get dataUtworzenia(): Date {
        return this._dataUtworzenia;
    }

    set dataUtworzenia(value: Date) {
        this._dataUtworzenia = value;
    }

    get komentarze(): Komentarz[] {
        return [...this._komentarze];
    }

    dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[] {
        return [...this._komentarze];
    }
}
