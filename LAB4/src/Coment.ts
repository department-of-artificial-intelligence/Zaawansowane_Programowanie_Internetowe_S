export class Coment {
    private _tresc: string
    private _date: Date
    private _odpowiedzi: Coment[]
    private _nick: string

    constructor(tresc: string, nick: string) {
        this._tresc = tresc
        this._nick = nick
    }

    get tresc() { return this._tresc }
    set tresc(tresc: string) { this._tresc = tresc }
    get data() { return this._date }
    set data(data: Date) { this._date = data }
    get nick() { return this._nick }
    set nick(nick: string) { this._nick = nick }

    dodajOdpowiedz(coment: Coment): void {
        this._odpowiedzi.push(coment)
    }

    pobierzOdpowiedzi(): Coment[] {
        let com: Coment[]
        this._odpowiedzi.forEach(o => com.push(o))
        return com
    }

}