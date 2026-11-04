import { Autor } from "./Autor";
import { Artykul } from "./Artykul";

export class Blog {
    private static nextId = 1;
    private _id: number;
    private _nazwa!: string;
    private _autor!: Autor;
    private _artykuly: Artykul[] = [];

    constructor(nazwa: string, autor: Autor) {
        this._id = Blog.nextId++;
        this.nazwa = nazwa;
        this.autor = autor;
    }

    get id(): number {
        return this._id;
    }

    get nazwa(): string {
        return this._nazwa;
    }

    set nazwa(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Nazwa bloga nie może być pusta.");
        }
        this._nazwa = value.trim();
    }

    get autor(): Autor {
        return this._autor;
    }

    set autor(value: Autor) {
        if (!value) {
            throw new Error("Autor bloga musi być podany.");
        }
        this._autor = value;
    }

    dodajArtykul(artykul: Artykul): void {
        if (!artykul) {
            throw new Error("Artykuł nie może być pusty.");
        }
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[] {
        return this._artykuly.map(artykul => artykul.tytul);
    }

    pobierzArtykul(tytul: string): Artykul | undefined {
        return this._artykuly.find(artykul => artykul.tytul === tytul);
    }
}