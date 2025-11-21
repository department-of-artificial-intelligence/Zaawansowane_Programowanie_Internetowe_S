import { Autor } from "./Autor";
import { IAuthorFormatter } from "./IAuthorFormatter";

// Konkretna strategia: formatowanie do listy HTML
export class HtmlListFormatter implements IAuthorFormatter {
    public format(autory: Autor[]): string {
        let html = "<ul>\n";

        // Iterujemy po każdym autorze i tworzymy element listy (li)
        for (const autor of autory) {
            html += `  <li>${autor.imie} ${autor.nazwisko} (${autor.email})</li>\n`;
        }

        html += "</ul>";
        return html;
    }
}