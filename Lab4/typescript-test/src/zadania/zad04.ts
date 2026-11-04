import { Licznik } from "../licznik/Licznik";
import { LicznikWidok } from "../licznik/LicznikWidok";
import { naglowek, type Zadanie } from "./Zadanie";

export const zadanie04: Zadanie = {
  numer: 4,
  tytul: "Licznik + / -",
  uruchom(kontener) {
    naglowek(kontener,
      "Model (Licznik) jest oddzielony od widoku (LicznikWidok). Widok obserwuje model, " +
      "więc ten sam licznik może mieć kilka widoków - oba poniżej pokazują tę samą wartość.");
    const licznik = new Licznik(0);
    new LicznikWidok(kontener, licznik);
    new LicznikWidok(kontener, licznik);
  },
};
