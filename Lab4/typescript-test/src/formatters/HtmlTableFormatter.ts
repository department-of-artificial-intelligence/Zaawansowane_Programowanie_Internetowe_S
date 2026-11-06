import { Autor } from "../domain/Autor";
import type { IAutorFormatter } from "./IAutorFormatter";

export class HtmlTableFormatter implements IAutorFormatter {
    format(autorzy: Autor[]): string {
        const rows = autorzy
            .map(
                (a) =>
                    `  <tr><td>${a.id}</td><td>${a.imie}</td><td>${a.nazwisko}</td><td>${a.email}</td></tr>`
            )
            .join("\n");

        return `<table>\n  <thead>\n    <tr><th>ID</th><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>\n  </thead>\n  <tbody>\n${rows}\n  </tbody>\n</table>`;
    }
}