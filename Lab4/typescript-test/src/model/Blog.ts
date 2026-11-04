import { Artykul } from "./Artykul";
import { Autor } from "./Autor";
import { BladWalidacji, wymagany, wymaganyObiekt } from "./walidacja";

export class Blog {
  private static nastepneId = 1;

  private _id: number;
  private _nazwa!: string;
  private _autor!: Autor;
  private _artykuly: Artykul[] = [];

  constructor(nazwa: string, autor: Autor) {
    this.nazwa = nazwa;   // Zadanie 2: nazwa nie może być pusta
    this.autor = autor;   // Zadanie 2: autor musi być podany
    this._id = Blog.nastepneId++;
  }

  get id(): number { return this._id; }

  get nazwa(): string { return this._nazwa; }
  set nazwa(value: string) { this._nazwa = wymagany(value, "nazwa"); }

  get autor(): Autor { return this._autor; }
  set autor(value: Autor) {
    const autor = wymaganyObiekt(value, "autor");
    // typy TypeScript znikają po kompilacji - w czasie działania sprawdza to instanceof
    if (!(autor instanceof Autor)) {
      throw new BladWalidacji('Pole "autor" musi być obiektem klasy Autor');
    }
    this._autor = autor;
  }

  dodajArtykul(artykul: Artykul): void {
    this._artykuly.push(wymaganyObiekt(artykul, "artykul"));
  }

  pobierzTytulyArtykulow(): string[] {
    return this._artykuly.map(artykul => artykul.tytul);
  }

  pobierzArtykul(tytul: string): Artykul | undefined {
    return this._artykuly.find(artykul => artykul.tytul === tytul);
  }
}
