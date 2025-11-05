import { Autor } from "./Autor";
import { IAutorFormatter } from "./IAutorFormatter";

export class HtmlTableFormatter implements IAutorFormatter {
    format(autorzy: Autor[]): string {
        let html = "<table><tr><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>";
        autorzy.forEach(a => {
            html += `<tr><td>${a.imie}</td><td>${a.nazwisko}</td><td>${a.email}</td></tr>`;
        });
        html += "</table>";
        return html;
    }
}