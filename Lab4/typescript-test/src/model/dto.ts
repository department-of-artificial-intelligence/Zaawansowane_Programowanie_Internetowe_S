// Zadanie 9: typy DTO wyprowadzone z klas za pomocą Omit i Pick
import { Artykul } from "./Artykul";
import { Autor } from "./Autor";
import { Blog } from "./Blog";

// Pomocniczy typ: tylko właściwości z danymi, bez metod klasy
// (dodajArtykul, aktualizuj, toString...). Prywatne pola i tak nie należą do keyof.
type TylkoDane<T> = {
  [K in keyof T as T[K] extends (...args: never[]) => unknown ? never : K]: T[K];
};

// DTO do tworzenia: bez identyfikatora i bez wartości wyliczanych przez obiekt
export type NowyAutor = Omit<TylkoDane<Autor>, "id" | "pelneImie">;
// { imie: string; nazwisko: string; email: string }

export type NowyBlog = Omit<TylkoDane<Blog>, "id">;
// { nazwa: string; autor: Autor }

export type NowyArtykul = Omit<TylkoDane<Artykul>, "id" | "dataUtworzenia">;
// { tytul: string; tresc: string }

// Dane do wyświetlenia autora na liście
export type AutorNaLiscie = Pick<Autor, "imie" | "nazwisko">;
// { imie: string; nazwisko: string }

export function utworzAutora(dto: NowyAutor): Autor {
  return new Autor(dto.imie, dto.nazwisko, dto.email);
}

export function utworzBlog(dto: NowyBlog): Blog {
  return new Blog(dto.nazwa, dto.autor);
}

export function utworzArtykul(dto: NowyArtykul): Artykul {
  return new Artykul(dto.tytul, dto.tresc);
}
