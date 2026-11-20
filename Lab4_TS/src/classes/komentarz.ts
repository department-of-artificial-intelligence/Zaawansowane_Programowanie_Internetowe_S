export class Komentarz{
    private _tresc: string;
    private _data: Date;
    private _nick: string;
    private _odpowiedzi: Komentarz[]

    get tresc(): string{
        return this._tresc;
    }
    set tresc(value: string){
        this._tresc = value;
    }

    get data(): Date{
        return this._data;
    }

    get nick(): string{
        return this._nick;
    }
    set nick(value: string){
        this._nick = value;
    }
    
    constructor(tresc: string, nick: string){
        this.tresc = tresc;
        this.nick = nick;
    }

    dodajOdpowiedz(odpowiedz: Komentarz): void{
        this._odpowiedzi.push(odpowiedz);
    }

    pobierzOdpowiedzi(): Komentarz[]{
        return this._odpowiedzi;
    }
}