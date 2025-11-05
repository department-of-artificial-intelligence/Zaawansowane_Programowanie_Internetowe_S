class Artykul {
    get tytul() {
        return this._tytul;
    }
    get tresc() {
        return this._tresc;
    }
    constructor(tytul, tresc) {
        this._dataUtworzenia = new Date;
        if (tytul.trim() === "") {
            throw "Tytul nie może być pusta.";
        }
        this._tytul = tytul;
        this._tresc = tresc;
    }
    dodajKomentarz(komentarz) {
        this._komentarze.push(komentarz);
    }
    pobierzKomentarze() {
        return this._komentarze;
    }
}
