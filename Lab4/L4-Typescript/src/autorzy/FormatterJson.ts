import { Autor } from "../blog/Autor";
import { IFormatterAutorow } from "./IFormatterAutorow";

export class FormatterJson implements IFormatterAutorow {
    formatuj(autorzy: Autor[]): string {
        return JSON.stringify(
            autorzy.map(autor => ({
                imie: autor.imie,
                nazwisko: autor.nazwisko,
                email: autor.email
            })),
            null,
            2);
    }
}
