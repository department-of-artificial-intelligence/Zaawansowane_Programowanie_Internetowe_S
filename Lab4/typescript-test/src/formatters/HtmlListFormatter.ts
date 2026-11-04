import { Author } from "../models/Author";
import type { AuthorFormatter } from "./AuthorFormatter";
import { escapeHtml } from "./escapeHtml";

export class HtmlListFormatter implements AuthorFormatter {
    private ordered: boolean;

    // ordered = false -> <ul> (bulleted), ordered = true -> <ol> (numbered)
    constructor(ordered: boolean = false) {
        this.ordered = ordered;
    }

    format(authors: Author[]): string {
        const tag = this.ordered ? "ol" : "ul";
        const items = authors.map(a => `<li>${escapeHtml(a.toString())}</li>`).join("");
        return `<${tag}>${items}</${tag}>`;
    }
}
