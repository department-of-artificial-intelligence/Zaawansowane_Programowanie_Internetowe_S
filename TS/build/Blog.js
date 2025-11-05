class Blog {
    get nazwa() {
        return this._nazwa;
    }
    get autor() {
        return this._autor;
    }
    constructor(nazwa, autor) {
        if (nazwa.trim() === "") {
            throw "Nazwa nie może być pusta.";
        }
        if (!autor) {
            throw "Autor musi być podany";
        }
        this._nazwa = nazwa;
        this._autor = autor;
    }
    dodajArtykul(artykul) {
        this._artykuly.push(artykul);
    }
    pobierzTytulyArtykulow() {
        return this._artykuly.map((a) => a.tytul);
    }
    pobierzArtykul(tytul) {
        return this._artykuly.find((a) => a.tytul == tytul);
    }
}
