export interface Zadanie {
  numer: number;
  tytul: string;
  uruchom(kontener: HTMLElement): void | Promise<void>;
}

export const ADRES_DANYCH = `${import.meta.env.BASE_URL}dane.json`;

export function naglowek(kontener: HTMLElement, tekst: string): void {
  const p = document.createElement("p");
  p.className = "opis";
  p.textContent = tekst;
  kontener.appendChild(p);
}
