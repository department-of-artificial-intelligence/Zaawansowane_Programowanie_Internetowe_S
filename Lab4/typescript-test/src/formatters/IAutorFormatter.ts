import { Autor } from "../domain/Autor";

export interface IAutorFormatter {
    format(autorzy: Autor[]): string;
}