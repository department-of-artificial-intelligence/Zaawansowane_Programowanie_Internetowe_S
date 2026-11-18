import { Autor } from "../blog/Autor";

export interface IFormatterAutorow {
    formatuj(autorzy: Autor[]): string;
}
