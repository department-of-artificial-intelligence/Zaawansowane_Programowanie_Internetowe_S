import type { Author } from "../classes/author";
import type { IAuthorExporter } from "../interfaces/IAuthorExporter";

export class HtmlTableExporter implements IAuthorExporter {
  export(authors: Author[]): string {
    const rows = authors
      .map(
        (a) =>
          `  <tr><td>${a.id}</td><td>${a.firstName}</td><td>${a.lastName}</td><td>${a.email}</td></tr>`
      )
      .join("\n");

    return `<table>
  <thead>
    <tr><th>ID</th><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>
  </thead>
  <tbody>
${rows}
  </tbody>
</table>`;
  }
}
