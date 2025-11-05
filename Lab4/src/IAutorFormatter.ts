import { Autor } from "./Autor";

export interface IAutorFormatter {
    format(autorzy: Autor[]): string;
}