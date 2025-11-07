class Blog {
    private _id: number;
    @require("Pole nazwa nie może być puste") private _nazwa: string;
    @require("Autor musi być podany") private _autor: Autor;
    private _artykuly: Artykul[];

    constructor(nazwa: string, autor: Autor) {
        this._nazwa = nazwa;
        this._autor = autor;
    }

    dodajArtykul(artykul: Artykul) : void {
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[] {
        let tytuly: string[] = [];

        this._artykuly.forEach((element) => tytuly.push(element.tytul));

        return tytuly;
    }

    pobierzArtykul(tytul: string) {
        return this._artykuly.find((element) => element.tytul === tytul);
    }
}