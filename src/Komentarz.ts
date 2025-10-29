export class Komentarz {
    private _tresc: string;
    private _date: Date;
    private _odpowiedzi: Komentarz[];
    private _nick: string;

    constructor(tresc: string, nick: string) {
        this._tresc = tresc;
        this._nick = nick;
    }

    get tresc() : string{ 
        return this._tresc;
    };
    set tresc(tresc: string) { 
        this._tresc = tresc; 
    };
    get data() : Date{ 
        return this._date; 
    };
    set data(data: Date) { 
        this._date = data; 
    };
    get nick() : string { 
        return this._nick;
    };
    set nick(nick: string) { 
        this._nick = nick;
    };

    dodajOdpowiedz(komentarz: Komentarz): void {
        this._odpowiedzi.push(komentarz)
    }

    pobierzOdpowiedzi(): Komentarz[] {
        let kom
        this._odpowiedzi.forEach(o => kom.push(o))
        return kom
    }

}