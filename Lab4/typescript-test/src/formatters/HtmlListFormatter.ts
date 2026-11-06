import { Autor } from "../domain/Autor";
import type { IAutorFormatter } from "./IAutorFormatter";

export class HtmlListFormatter implements IAutorFormatter {
    format(autorzy: Autor[]): string {
        const items = autorzy
            .map((a) => `  <li>${a.imie} ${a.nazwisko} (${a.email})</li>`)
            .join("\n");

        return `<ul>\n${items}\n</ul>`;
    }
}