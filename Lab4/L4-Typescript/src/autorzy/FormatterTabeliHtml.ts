import { Autor } from "../blog/Autor";
import { IFormatterAutorow } from "./IFormatterAutorow";
import { ucieknijHtml } from "./ucieknijHtml";

export class FormatterTabeliHtml implements IFormatterAutorow {
    formatuj(autorzy: Autor[]): string {
        const wiersze = autorzy
            .map(autor =>
                `<tr><td>${ucieknijHtml(autor.imie)}</td>` +
                `<td>${ucieknijHtml(autor.nazwisko)}</td>` +
                `<td>${ucieknijHtml(autor.email)}</td></tr>`)
            .join("");
        return `<table border="1"><tr><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>${wiersze}</table>`;
    }
}
