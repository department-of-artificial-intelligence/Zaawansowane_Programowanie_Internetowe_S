import type { ReadonlyAuthors } from "../blog/types";
import type { IAuthorFormatter } from "./IAuthorFormatter";

export class AuthorExporter {
    private formatter: IAuthorFormatter;

    constructor(formatter: IAuthorFormatter) {
        this.formatter = formatter;
    }

    setFormatter(formatter: IAuthorFormatter): void {
        this.formatter = formatter;
    }

    exportAuthors(authors: ReadonlyAuthors): string {
        return this.formatter.format(authors);
    }
}
