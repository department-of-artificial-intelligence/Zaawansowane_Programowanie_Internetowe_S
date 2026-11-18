import { Artykul } from "./Artykul";
import { Autor } from "./Autor";

export class Blog {
    private static nastepneId: number = 1;

    private _id: number = Blog.nastepneId++;
    private _nazwa!: string;
    private _autor!: Autor;
    private _artykuly: Artykul[] = [];

    constructor(nazwa: string, autor: Autor) {
        this.nazwa = nazwa;
        this.autor = autor;
    }

    get nazwa(): string { return this._nazwa; }
    set nazwa(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Nazwa bloga nie może być pusta");
        this._nazwa = value;
    }

    get autor(): Autor { return this._autor; }
    set autor(value: Autor) {
        if (value === null || value === undefined)
            throw new Error("Autor bloga musi być podany");
        this._autor = value;
    }

    dodajArtykul(artykul: Artykul): void {
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[] {
        return this._artykuly.map(artykul => artykul.tytul);
    }

    pobierzArtykul(tytul: string): Artykul | undefined {
        return this._artykuly.find(artykul => artykul.tytul === tytul);
    }
}
