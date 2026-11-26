import type { Author } from "../classes/author";
import type { IAuthorExporter } from "../interfaces/IAuthorExporter";

export class HtmlListExporter implements IAuthorExporter {
  export(authors: Author[]): string {
    const items = authors
      .map((a) => `  <li>${a.firstName} ${a.lastName} (${a.email})</li>`)
      .join("\n");

    return `<ul>\n${items}\n</ul>`;
  }
}
