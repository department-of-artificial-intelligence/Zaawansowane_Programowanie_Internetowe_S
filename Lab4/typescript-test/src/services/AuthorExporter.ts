import {
    type IAuthorFormatter,
    type ReadonlyAuthor,
    HtmlTableAuthorFormatter,
    HtmlListAuthorFormatter,
    HtmlOrderedListAuthorFormatter, // <-- Nowy import
    JsonAuthorFormatter
} from "./AuthorFormatters";

export class AuthorExporter {
    private _formatter: IAuthorFormatter;

    constructor(formatter: IAuthorFormatter) {
        this._formatter = formatter;
    }

    setFormatter(formatter: IAuthorFormatter): void {
        this._formatter = formatter;
    }

    // Zadanie 12: Zabezpieczenie danych typem readonly
    export(authors: readonly ReadonlyAuthor[]): string {
        return this._formatter.format(authors);
    }
}

// Rozszerzenie unii typów o "ordered-list"
export type ExportFormatType = "table" | "list" | "ordered-list" | "json";

export class AuthorFormatterFactory {
    static create(format: ExportFormatType): IAuthorFormatter {
        switch (format) {
            case "table":
                return new HtmlTableAuthorFormatter();
            case "list":
                return new HtmlListAuthorFormatter();
            case "ordered-list":
                return new HtmlOrderedListAuthorFormatter();
            case "json":
                return new JsonAuthorFormatter();
            default:
                throw new Error(`Unsupported format type: ${format}`);
        }
    }
}