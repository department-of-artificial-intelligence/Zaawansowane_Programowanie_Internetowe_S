import type { Author } from "../classes/author";
import { measureTime } from "../decorators/measureTime";
import type { IAuthorExporter } from "../interfaces/IAuthorExporter";

export class AuthorController {
  private exporter: IAuthorExporter;

  constructor(exporter: IAuthorExporter) {
    this.exporter = exporter;
  }

  setExporter(exporter: IAuthorExporter): void {
    this.exporter = exporter;
  }
  @measureTime
  getAuthorsOutput(authors: Author[]): string {
    return this.exporter.export(authors);
  }
}
