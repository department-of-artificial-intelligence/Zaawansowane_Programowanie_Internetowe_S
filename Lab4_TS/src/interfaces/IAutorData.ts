import { Autor } from "../classes/autor";

export interface IAutorFormatter{
    format(autor: Autor): string;
}
