import { Author } from "../models/Author";
import type { AuthorFormatter } from "./AuthorFormatter";

// Context of the Strategy pattern - the strategy can be swapped at runtime
export class AuthorExporter {
    private formatter: AuthorFormatter;

    constructor(formatter: AuthorFormatter) {
        this.formatter = formatter;
    }

    setFormatter(formatter: AuthorFormatter): void {
        this.formatter = formatter;
    }

    export(authors: Author[]): string {
        return this.formatter.format(authors);
    }
}
