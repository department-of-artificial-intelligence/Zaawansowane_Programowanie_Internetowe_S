import { Autor } from './Autor';
import { Arttykul } from './Artykul';

export class Blog {
    private _id: number;
    private _nazwa: string;
    private _autor: Autor;
    private _artykuly: Arttykul[];

    constructor(nazwa: string, autor: Autor) {
        if (!nazwa?.trim()) throw new Error("Nazwa bloga nie może być pusta.");
        if (!autor) throw new Error("Autor jest wymagany.");

        this._nazwa = nazwa.trim();
        this._autor = autor;
        this._artykuly = [];
        this._id = 0;
    }

    get nazwa(): string {
        return this._nazwa;
    }

    set nazwa(value: string) {
        if (!value?.trim()) throw new Error("Nazwa bloga nie może być pusta.");
        this._nazwa = value.trim();
    }

    get autor(): Autor {
        return this._autor;
    }

    set autor(value: Autor) {
        if (!value) throw new Error("Autor jest wymagany.");
        this._autor = value;
    }

    dodajArtykul(artykul: Arttykul): void {
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[] {
        return this._artykuly.map(a => a.tytul);
    }
}
