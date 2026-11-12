import type { IAutorFormatter } from "./IAutorFormatter";
import { HtmlTableFormatter } from "./HtmlTableFormatter";
import { HtmlListFormatter } from "./HtmlListFormatter";
import { JsonFormatter } from "./JsonFormatter";

export type FormatType = "html-table" | "html-list" | "json";

export class AutorFormatterFactory {
    static createFormatter(type: FormatType): IAutorFormatter {
        switch (type) {
            case "html-table":
                return new HtmlTableFormatter();
            case "html-list":
                return new HtmlListFormatter();
            case "json":
                return new JsonFormatter();
            default:
                throw new Error(`Nieobsługiwany format: ${type}`);
        }
    }
}