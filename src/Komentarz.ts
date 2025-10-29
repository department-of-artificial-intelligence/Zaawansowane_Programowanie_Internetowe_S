export class Komentarz {
    private _tresc: string;
    private _data: Date;
    private _nick: string;

    constructor(tresc: string, nick: string, data?: Date) {
        this._tresc = tresc;
        this._nick = nick;
        this._data = data || new Date();
    }

    get tresc(): string {
        return this._tresc;
    }

    set tresc(value: string) {
        this._tresc = value;
    }

    get data(): Date {
        return this._data;
    }

    set data(value: Date) {
        this._data = value;
    }

    get nick(): string {
        return this._nick;
    }

    set nick(value: string) {
        this._nick = value;
    }
    dodajOdpowiedz(odpowiedz: Komentarz): void {

        console.log(`Dodano odpowiedź "${odpowiedz.tresc}" do komentarza "${this.tresc}"`);
    }

    pobierzOdpowiedzi(): Komentarz[] {
        return [];
    }
}
