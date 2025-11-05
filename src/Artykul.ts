import {Komentarz} from "./Komentarz"

export class Artykul{
    private _id:number;
    private _tytul:string;
    private _tresc:string;
    private _dataUtworzenia:Date;
    private _komentarze:Komentarz[];

    constructor(tytul:string,tresc:string){
        if (!tytul.trim()) throw new Error("Pole tytul nie może być puste");
        this._tytul = tytul;
        this._tresc = tresc;
    }

    
    get tytul() : string {
        return this._tytul;
    }
    
    set tytul(value : string) {
        this._tytul = value;
    }

    get tresc() : string {
        return this._tresc;
    }

    set resc(value : string) {
        this._tresc = value;
    }
    
    dodajKomentarz(komentarz: Komentarz): void {
        this._komentarze.push(komentarz)
    }

    pobierzKomentarze(): Komentarz[] {
        let kom
        this._komentarze.forEach(c => kom.push(c))
        return kom;
    }
}