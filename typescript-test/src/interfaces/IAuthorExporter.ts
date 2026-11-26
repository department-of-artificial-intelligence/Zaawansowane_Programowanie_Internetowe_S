import type { Author } from "../classes/author";

export interface IAuthorExporter {
  export(authors: Author[]): string;
}
