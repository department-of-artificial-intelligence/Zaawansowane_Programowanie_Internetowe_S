import { Author } from "../models/Author";
import type { AuthorFormatter } from "./AuthorFormatter";
import { escapeHtml } from "./escapeHtml";

export class HtmlTableFormatter implements AuthorFormatter {
    format(authors: Author[]): string {
        const rows = authors.map(a =>
            `<tr><td>${a.id}</td><td>${escapeHtml(a.firstName)}</td>` +
            `<td>${escapeHtml(a.lastName)}</td><td>${escapeHtml(a.email)}</td></tr>`
        ).join("");
        return `<table border="1"><thead><tr><th>Id</th><th>Imię</th><th>Nazwisko</th><th>E-mail</th></tr></thead>` +
            `<tbody>${rows}</tbody></table>`;
    }
}
