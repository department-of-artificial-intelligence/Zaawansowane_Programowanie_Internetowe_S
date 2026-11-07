import { Author } from "../models/Author";

// Zadanie 12: Alias dla autora zabezpieczonego typem Readonly
export type ReadonlyAuthor = Readonly<Author>;

export interface IAuthorFormatter {
    // Zabezpieczenie przed mutacj¹ tablicy (readonly) oraz w³aœciwoœci obiektów (ReadonlyAuthor)
    format(authors: readonly ReadonlyAuthor[]): string;
}

// 1. Strategia: Tabela HTML
export class HtmlTableAuthorFormatter implements IAuthorFormatter {
    format(authors: readonly ReadonlyAuthor[]): string {
        const rows = authors
            .map(
                (author) => `
        <tr>
          <td>${author.id ?? "-"}</td>
          <td>${author.firstName}</td>
          <td>${author.lastName}</td>
          <td><a href="mailto:${author.email}">${author.email}</a></td>
        </tr>`
            )
            .join("");

        return `
      <table border="1" cellpadding="8" style="border-collapse: collapse; width: 100%; margin-top: 12px;">
        <thead>
          <tr style="background-color: #f2f2f2; text-align: left;">
            <th>ID</th>
            <th>Imiê</th>
            <th>Nazwisko</th>
            <th>Email</th>
          </tr>
        </thead>
        <tbody>
          ${rows}
        </tbody>
      </table>`;
    }
}

// 2. Strategia: Lista HTML (punktowana - UL)
export class HtmlListAuthorFormatter implements IAuthorFormatter {
    format(authors: readonly ReadonlyAuthor[]): string {
        const listItems = authors
            .map(
                (author) =>
                    `<li><strong>#${author.id ?? "-"} ${author.firstName} ${author.lastName}</strong> (<a href="mailto:${author.email}">${author.email}</a>)</li>`
            )
            .join("");

        return `<ul style="line-height: 1.8; margin-top: 12px;">${listItems}</ul>`;
    }
}

// 3. Strategia: Lista HTML (numerowana - OL) <-- NOWOŒÆ DLA ZADANIA 7
export class HtmlOrderedListAuthorFormatter implements IAuthorFormatter {
    format(authors: readonly ReadonlyAuthor[]): string {
        const listItems = authors
            .map(
                (author) =>
                    `<li><strong>${author.firstName} ${author.lastName}</strong> — <a href="mailto:${author.email}">${author.email}</a></li>`
            )
            .join("");

        return `<ol style="line-height: 1.8; margin-top: 12px;">${listItems}</ol>`;
    }
}

// 4. Strategia: Dokument JSON
export class JsonAuthorFormatter implements IAuthorFormatter {
    format(authors: readonly ReadonlyAuthor[]): string {
        const data = authors.map((author) => ({
            id: author.id,
            firstName: author.firstName,
            lastName: author.lastName,
            email: author.email,
        }));

        return JSON.stringify(data, null, 2);
    }
}