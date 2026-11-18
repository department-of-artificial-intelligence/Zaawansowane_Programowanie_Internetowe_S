import { Autor } from "../blog/Autor";
import { IFormatterAutorow } from "./IFormatterAutorow";
import { ucieknijHtml } from "./ucieknijHtml";

export class FormatterListyHtml implements IFormatterAutorow {
    constructor(private znacznik: "ul" | "ol") { }

    formatuj(autorzy: Autor[]): string {
        const elementy = autorzy
            .map(autor =>
                `<li>${ucieknijHtml(autor.imie)} ${ucieknijHtml(autor.nazwisko)} ` +
                `(${ucieknijHtml(autor.email)})</li>`)
            .join("");
        return `<${this.znacznik}>${elementy}</${this.znacznik}>`;
    }
}
