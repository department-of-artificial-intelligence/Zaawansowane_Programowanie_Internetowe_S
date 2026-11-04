import type { AuthorFormatter } from "./AuthorFormatter";
import { HtmlTableFormatter } from "./HtmlTableFormatter";
import { HtmlListFormatter } from "./HtmlListFormatter";
import { JsonFormatter } from "./JsonFormatter";
import { CsvFormatter } from "./CsvFormatter";

export type FormatType = "table" | "ul" | "ol" | "json" | "csv";

// Simple Factory - creates the right strategy by name
export class FormatterFactory {
    static create(type: FormatType): AuthorFormatter {
        switch (type) {
            case "table": return new HtmlTableFormatter();
            case "ul": return new HtmlListFormatter(false);
            case "ol": return new HtmlListFormatter(true);
            case "json": return new JsonFormatter();
            case "csv": return new CsvFormatter();
        }
    }
}
