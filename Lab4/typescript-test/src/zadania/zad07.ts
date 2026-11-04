import { AutorService } from "../autorzy/AutorService";
import { WidokAutorow } from "../autorzy/WidokAutorow";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

export const zadanie07: Zadanie = {
  numer: 7,
  tytul: "Wybór formy wyświetlania",
  async uruchom(kontener) {
    naglowek(kontener,
      "Forma wybierana z listy rozwijanej. Każda forma to strategia z zadania 3, " +
      "więc widok nie zawiera żadnego kodu typu if (forma === ...).");
    const widok = new WidokAutorow(kontener, ["punktowana", "numerowana", "tabela"], "punktowana");
    const serwis = new AutorService(ADRES_DANYCH);
    try {
      await serwis.wczytaj();
      widok.pokaz(serwis.pobierzWszystkich());
    } catch (blad) {
      new Konsola(kontener).blad(blad);
    }
  },
};
