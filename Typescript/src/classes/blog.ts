import { Artykul } from "./artykul";
import { Autor } from "./autor";

export class Blog {
    private _id: number;
    private _nazwa: string
    private _autor: Autor;
    private _artykuly: Artykul[];

    get nazwa(): string { return this._nazwa }
    set nazwa(value: string) { this._nazwa = value; }

    get autor(): Autor { return this._autor }
    set autor(value: Autor) { this._autor = value; }

    constructor(nazwa: string, autor: Autor) {
        this._nazwa = nazwa;
        this._autor = autor;
        this._id = 0;
        this._artykuly = [];
    }
    dodajArtykul(artykul: Artykul): void {
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[] {
        let nazwy: string[] = [];
        this._artykuly.forEach(element => {
            nazwy.push(element.tytul);
        });
        return nazwy;
    }

    pobierzArtykul(tytul: string): Artykul | undefined {
        return this._artykuly.find(a => a.tytul == tytul);
    }
}