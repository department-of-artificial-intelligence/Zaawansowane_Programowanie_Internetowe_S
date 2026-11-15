import { Autor } from './author.ts';
import { CreateAutorDTO } from './dtos.ts';

export class AutorUpdater {
  public static aktualizuj(autor: Autor, dane: Partial<CreateAutorDTO>): Autor {
    if (dane.imie !== undefined) {
      autor.imie = dane.imie;
    }
    if (dane.nazwisko !== undefined) {
      autor.nazwisko = dane.nazwisko;
    }
    if (dane.email !== undefined) {
      autor.email = dane.email;
    }
    return autor;
  }
}

// Example:
// AutorUpdater.aktualizuj(autor, { email: 'nowy.adres@example.com' });