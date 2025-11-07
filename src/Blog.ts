import { Autor } from "./Autor";
import { Artykul } from "./Artykul";

export class Blog {
  private _id: number;
  private _nazwa: string;
  private _autor: Autor;
  private _artykuly: Artykul[] = [];

  constructor(nazwa: string, autor: Autor) {
    this._id = Date.now();
    this._nazwa = nazwa;
    this._autor = autor;
  }

  get nazwa(): string {
    return this._nazwa;
  }

  get autor(): Autor {
    return this._autor;
  }

  dodajArtykul(artykul: Artykul): void {
    this._artykuly.push(artykul);
  }

  pobierzTytulyArtykulow(): string[] {
    return this._artykuly.map(a => a.tytul);
  }

  pobierzArtykul(tytul: string): Artykul | undefined {
    return this._artykuly.find(a => a.tytul === tytul);
  }
}
