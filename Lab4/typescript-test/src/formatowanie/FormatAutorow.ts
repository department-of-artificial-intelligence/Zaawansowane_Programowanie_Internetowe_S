import type { Autor } from "../model/Autor";

// Zadanie 3: wzorzec Strategia. Każdy format to osobna klasa z tym samym interfejsem,
// a kod korzystający z formatów nie zna ich szczegółów.
// Zadanie 12: formaty dostają dane tylko do odczytu.
export interface FormatAutorow {
  readonly nazwa: string;
  readonly typMime: string;
  formatuj(autorzy: readonly Readonly<Autor>[]): string;
}
