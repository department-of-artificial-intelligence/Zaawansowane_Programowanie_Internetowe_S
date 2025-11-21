import { Autor } from "./Autor";
import { IAuthorFormatter } from "./IAuthorFormatter";

// Konkretna strategia: formatowanie do tabeli HTML
export class HtmlTableFormatter implements IAuthorFormatter {
    public format(autory: Autor[]): string {
        // Budujemy stringa HTML
        let html = "<table>\n";
        html += "  <thead>\n";
        html += "    <tr><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>\n";
        html += "  </thead>\n";
        html += "  <tbody>\n";

        // Iterujemy po każdym autorze i tworzymy wiersz tabeli (tr)
        for (const autor of autory) {
            html += `    <tr><td>${autor.imie}</td><td>${autor.nazwisko}</td><td>${autor.email}</td></tr>\n`;
        }

        html += "  </tbody>\n";
        html += "</table>";

        return html;
    }
}