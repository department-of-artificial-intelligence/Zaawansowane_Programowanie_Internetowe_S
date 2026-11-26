import { Author } from "../classes/author";
import type { IAuthorExporter } from "../interfaces/IAuthorExporter";

export class HtmlNumberedListExporter implements IAuthorExporter {
  export(authors: Author[]): string {
    const items = authors
      .map((a) => `  <li>${a.firstName} ${a.lastName} (${a.email})</li>`)
      .join("\n");

    return `<ol>\n${items}\n</ol>`;
  }
}
