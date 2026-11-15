import { Autor } from './author.ts';
import { Artykul } from './article.ts';

export class Blog {
  private static _nextId = 1;

  private _id: number;
  private _nazwa: string;
  private _autor: Autor;
  private _artykuly: Artykul[];

  constructor(nazwa: string, autor: Autor) {
    this._id = Blog._nextId++;
    this._nazwa = nazwa;
    this._autor = autor;
    this._artykuly = [];
  }

  public get nazwa(): string {
    return this._nazwa;
  }

  public set nazwa(value: string) {
    if (!value || value.trim().length === 0){
        throw new Error("Nazwa nie może być pusta");
    }
    this._nazwa = value;
  }

  public get autor(): Autor {
    return this._autor;
  }

  public set autor(value: Autor) {
    if (!value || !(value instanceof Autor)){
        throw new Error ('Autor musi być podany');
    }
    this._autor = value;
  }

  public dodajArtykul(artykul: Artykul): void {
    this._artykuly.push(artykul);
  }

  public pobierzTytulyArtykulow(): string[] {
    return this._artykuly.map((artykul) => artykul.tytul);
  }

  public pobierzArtykul(tytul: string): Artykul | undefined {
    return this._artykuly.find((artykul) => artykul.tytul === tytul);
  }
}