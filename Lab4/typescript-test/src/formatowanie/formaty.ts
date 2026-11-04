import type { Autor } from "../model/Autor";
import { escapeHtml } from "../ui/html";
import type { FormatAutorow } from "./FormatAutorow";

export class TabelaHtml implements FormatAutorow {
  readonly nazwa = "Tabela";
  readonly typMime = "text/html";

  formatuj(autorzy: readonly Readonly<Autor>[]): string {
    const wiersze = autorzy
      .map(a => `<tr><td>${a.id}</td><td>${escapeHtml(a.imie)}</td>` +
                `<td>${escapeHtml(a.nazwisko)}</td><td>${escapeHtml(a.email)}</td></tr>`)
      .join("");
    return `<table><thead><tr><th>Id</th><th>Imię</th><th>Nazwisko</th><th>E-mail</th></tr></thead>` +
           `<tbody>${wiersze}</tbody></table>`;
  }
}

export class ListaHtml implements FormatAutorow {
  readonly nazwa: string;
  readonly typMime = "text/html";
  private readonly numerowana: boolean;

  constructor(numerowana: boolean) {
    this.numerowana = numerowana;
    this.nazwa = numerowana ? "Lista numerowana" : "Lista punktowana";
  }

  formatuj(autorzy: readonly Readonly<Autor>[]): string {
    const znacznik = this.numerowana ? "ol" : "ul";
    const elementy = autorzy
      .map(a => `<li>${escapeHtml(a.pelneImie)} &lt;${escapeHtml(a.email)}&gt;</li>`)
      .join("");
    return `<${znacznik}>${elementy}</${znacznik}>`;
  }
}

export class FormatJson implements FormatAutorow {
  readonly nazwa = "JSON";
  readonly typMime = "application/json";

  formatuj(autorzy: readonly Readonly<Autor>[]): string {
    // jawne mapowanie: JSON.stringify(autor) dałby prywatne pola _imie, _email...
    const dane = autorzy.map(a => ({ id: a.id, imie: a.imie, nazwisko: a.nazwisko, email: a.email }));
    return JSON.stringify(dane, null, 2);
  }
}

export class FormatCsv implements FormatAutorow {
  readonly nazwa = "CSV";
  readonly typMime = "text/csv";

  formatuj(autorzy: readonly Readonly<Autor>[]): string {
    const pole = (tekst: string) => `"${tekst.replaceAll('"', '""')}"`;
    const wiersze = autorzy.map(a => [String(a.id), a.imie, a.nazwisko, a.email].map(pole).join(";"));
    return ["id;imie;nazwisko;email", ...wiersze].join("\n");
  }
}
