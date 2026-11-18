import type { ReadonlyAuthors } from "../blog/types";
import { escapeHtml } from "./escapeHtml";
import type { IAuthorFormatter } from "./IAuthorFormatter";

export class HtmlTableFormatter implements IAuthorFormatter {
    format(authors: ReadonlyAuthors): string {
        const rows = authors
            .map(author =>
                `<tr><td>${escapeHtml(author.firstName)}</td>` +
                `<td>${escapeHtml(author.lastName)}</td>` +
                `<td>${escapeHtml(author.email)}</td></tr>`)
            .join("");
        return `<table border="1"><tr><th>First name</th><th>Last name</th><th>Email</th></tr>${rows}</table>`;
    }
}
