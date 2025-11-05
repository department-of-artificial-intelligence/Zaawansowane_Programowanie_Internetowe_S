class Blog {
  _id: number;
  _nazwa: string;
  _autor: Autor;
  _artykuly: Artykul[];

  get nazwa(): string {
    return this._nazwa;
  }
  get autor(): Autor {
    return this._autor;
  }

  constructor(nazwa: string, autor: Autor) {
    if (nazwa.trim() === "") {
      throw "Nazwa nie może być pusta.";
    }
    if (!autor) {
      throw "Autor musi być podany";
    }
    this._nazwa = nazwa;
    this._autor = autor;
  }

  dodajArtykul(artykul: Artykul): void {
    this._artykuly.push(artykul);
  }

  pobierzTytulyArtykulow(): string[] {
    return this._artykuly.map((a) => a.tytul);
  }

  pobierzArtykul(tytul: string): Artykul | undefined {
    return this._artykuly.find((a) => a.tytul == tytul);
  }
}
