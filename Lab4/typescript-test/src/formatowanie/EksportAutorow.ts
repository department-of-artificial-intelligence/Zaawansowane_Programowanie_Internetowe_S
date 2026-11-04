import type { Autor } from "../model/Autor";
import type { FormatAutorow } from "./FormatAutorow";
import { FormatCsv, FormatJson, ListaHtml, TabelaHtml } from "./formaty";

// Kontekst wzorca Strategia: przechowuje aktualną strategię i można ją podmienić w locie
export class EksportAutorow {
  private format: FormatAutorow;

  constructor(format: FormatAutorow) {
    this.format = format;
  }

  ustawFormat(format: FormatAutorow): void {
    this.format = format;
  }

  eksportuj(autorzy: readonly Readonly<Autor>[]): string {
    return this.format.formatuj(autorzy);
  }
}

// Fabryka/rejestr dostępnych formatów (Record z zadania 11 w innym zastosowaniu).
// Dodanie nowego formatu = nowa klasa + jeden wpis tutaj, bez zmian w reszcie kodu.
export const FORMATY = {
  punktowana: new ListaHtml(false),
  numerowana: new ListaHtml(true),
  tabela: new TabelaHtml(),
  json: new FormatJson(),
  csv: new FormatCsv(),
} satisfies Record<string, FormatAutorow>;

export type NazwaFormatu = keyof typeof FORMATY;
