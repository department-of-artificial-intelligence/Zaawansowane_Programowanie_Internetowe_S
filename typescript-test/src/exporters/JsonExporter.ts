import type { Author } from "../classes/author";
import type { IAuthorExporter } from "../interfaces/IAuthorExporter";

export class JsonExporter implements IAuthorExporter {
  export(authors: Author[]): string {
    const plainData = authors.map((a) => ({
      id: a.id,
      firstName: a.firstName,
      lastName: a.lastName,
      email: a.email,
    }));

    return JSON.stringify(plainData, null, 2);
  }
}
