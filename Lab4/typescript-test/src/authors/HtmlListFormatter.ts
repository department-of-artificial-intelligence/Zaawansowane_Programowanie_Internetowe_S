import type { ReadonlyAuthors } from "../blog/types";
import { escapeHtml } from "./escapeHtml";
import type { IAuthorFormatter } from "./IAuthorFormatter";

export class HtmlListFormatter implements IAuthorFormatter {
    private tag: "ul" | "ol";

    constructor(tag: "ul" | "ol") {
        this.tag = tag;
    }

    format(authors: ReadonlyAuthors): string {
        const items = authors
            .map(author =>
                `<li>${escapeHtml(author.firstName)} ${escapeHtml(author.lastName)} ` +
                `(${escapeHtml(author.email)})</li>`)
            .join("");
        return `<${this.tag}>${items}</${this.tag}>`;
    }
}
