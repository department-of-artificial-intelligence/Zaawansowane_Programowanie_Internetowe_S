import { Autor } from "./Autor";

// Nasz interfejs Strategii
// Definiuje jedną metodę, którą musi mieć każdy konkretny formater.
export interface IAuthorFormatter {
    /**
     * Formatuje listę autorów do określonego formatu (np. JSON, HTML).
     * @param autory Lista obiektów Autor do sformatowania.
     * @returns Sformatowany string.
     */
    format(autory: Autor[]): string;
}