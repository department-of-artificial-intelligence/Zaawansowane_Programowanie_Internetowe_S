class Komentarz {
    get tresc() {
        return this._tresc;
    }
    get data() {
        return this._data;
    }
    get nick() {
        return this._nick;
    }
    constructor(tresc, nick) {
        this._tresc = tresc;
        this._nick = nick;
    }
    dodajOdpowiedz(odpowiedz) {
        this._odpowiedzi.push(odpowiedz);
    }
    pobierzOdpowiedzi() {
        return this._odpowiedzi;
    }
}
