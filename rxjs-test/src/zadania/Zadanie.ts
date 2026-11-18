import type { Subscription } from "rxjs";

export interface Zadanie {
  id: string;
  tytul: string;
  opis: string;
  // Zwraca subskrypcję, którą aplikacja odpina przy przejściu do innej zakładki
  uruchom(kontener: HTMLElement): Subscription | void;
}
