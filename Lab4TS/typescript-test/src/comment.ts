export class Komentarz {
    private _tresc: string;
    private _data: Date;
    private _odpowiedzi: Komentarz[]
    private _nick: string;

    constructor(tresc: string, nick: string){
        this._tresc = tresc;
        this._nick = nick;
        this._odpowiedzi = [];
        this._data = new Date();
    }

    public get tresc(): string {
        return this._tresc;
    }

    public set tresc(value: string){
        this._tresc = value;
    }

    public get data(): Date {
        return this._data;
    }

    public set data(value: Date){
        this._data = value;
    }

    public get nick(): string {
        return this._nick;
    }

    public set nick(value: string){
        this._nick = value;
    }

    public dodajOdpowiedz(odpowiedz: Komentarz): void {
        this._odpowiedzi.push(odpowiedz);
    }

    public pobierzOdpowiedzi(): Komentarz[]{
        return [...this._odpowiedzi];
    }
}