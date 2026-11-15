import type { Komentarz } from "./comment";

export class Artykul {
    private static _nextId = 1;

    private _id:number;
    private _tytul:string;
    private _tresc:string;
    private _dataUtworzenia:Date;
    private _komentarze: Komentarz[]

    constructor(tytul: string, tresc: string){
        this._id = Artykul._nextId++;
        this._tytul = tytul;
        this._tresc = tresc;
        this._dataUtworzenia = new Date();
        this._komentarze = [];
    }


    public get tytul(): string{
        return this._tytul;
    }

    public set tytul(value: string){
        if(!value || value.trim().length === 0){
            throw new Error("Tytul nie może być pusty");
        }
        this._tytul = value;
    }

    public get tresc(): string {
        return this._tresc;
    }

    public set tresc(value: string){
        this._tresc = value;
    }

    public dodajKomentarz(komentarz: Komentarz): void{
        this._komentarze.push(komentarz);
    }

    public pobierzKomentarze(): Komentarz[]{
        return [...this._komentarze];
    }

}