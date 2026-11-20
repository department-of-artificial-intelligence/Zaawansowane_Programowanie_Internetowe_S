import { Komentarz } from "./komentarz";

export class Artykul{
    private _id: number;
    private _tytul: string;
    private _tresc: string;
    private _dataUtworzenia: Date;
    private _komentarze: Komentarz[];

    get tytul(): string{
        return this._tytul;
    }
    set tytul(value: string){
        const clean = value.trim();
        if(!clean)
            throw new Error("Tytul jest wymagany !");
        this._tytul = clean;
    }

    get tresc(): string{
        return this._tresc;
    }
    set tresc(value: string){
        this._tresc = value;
    }

    constructor(tytul: string, tresc: string, id: number = 0){
        this._id = id;
        this._tytul = "";
        this.tytul = tytul;
        this.tresc = tresc;
        this._dataUtworzenia = new Date();
    }

    dodajKomentarz(komentarz: Komentarz): void{
        this._komentarze.push(komentarz);
    }

    pobierzKomentarze(): Komentarz[]{
        return this._komentarze;
    }
}