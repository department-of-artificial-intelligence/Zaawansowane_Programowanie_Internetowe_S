import { measureTime } from "../dekoratory/measureTime";
import { Autor } from "../model/Autor";
import type { NowyAutor } from "../model/dto";
import type { AutorDto } from "./AutorDto";

export interface WynikWczytywania {
  wczytano: number;
  bledy: string[];
}

export class AutorService {
  private readonly url: string;

  // Zadanie 11: autorzy w strukturze Record indeksowanej identyfikatorem
  private autorzy: Record<number, Autor> = {};

  constructor(url: string) {
    this.url = url;
  }

  // Zadanie 5: asynchroniczne pobranie listy i zamiana DTO na obiekty klasy Autor.
  // Zadanie 14: czas wczytywania mierzy dekorator.
  @measureTime
  async wczytaj(): Promise<WynikWczytywania> {
    const odpowiedz = await fetch(this.url);
    if (!odpowiedz.ok) {
      throw new Error(`Nie udało się pobrać ${this.url} (HTTP ${odpowiedz.status})`);
    }
    const dane = await (odpowiedz.json() as Promise<AutorDto[]>);

    const autorzy: Record<number, Autor> = {};
    const bledy: string[] = [];
    for (const dto of dane) {
      try {
        // konstruktor waliduje dane (zadanie 2), więc błędny rekord nie trafi do listy
        autorzy[dto.id] = new Autor(dto.imie, dto.nazwisko, dto.email, dto.id);
      } catch (blad) {
        bledy.push(`Rekord id=${dto.id}: ${(blad as Error).message}`);
      }
    }

    this.autorzy = autorzy;
    return { wczytano: Object.keys(autorzy).length, bledy };
  }

  // Zadanie 12: na zewnątrz tylko dane do odczytu
  pobierzWszystkich(): readonly Readonly<Autor>[] {
    return Object.values(this.autorzy);
  }

  // Zadania 11 i 13: dostęp po kluczu (bez przeszukiwania tablicy);
  // brak autora -> undefined zamiast wyjątku
  znajdzPoId(id: number): Readonly<Autor> | undefined {
    return this.autorzy[id];
  }

  // Zadanie 10: częściowa aktualizacja wybranego autora
  aktualizujAutora(id: number, zmiany: Partial<NowyAutor>): Readonly<Autor> | undefined {
    const autor = this.autorzy[id];
    autor?.aktualizuj(zmiany);
    return autor;
  }
}
