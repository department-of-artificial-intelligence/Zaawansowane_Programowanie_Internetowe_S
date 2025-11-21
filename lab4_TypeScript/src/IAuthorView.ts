import { Autor } from "./Autor";

/**
 * Interfejs Strategii dla renderowania widoku autorów.
 */
export interface IAuthorView {
    /**
     * Renderuje listę autorów wewnątrz podanego kontenera.
     * @param container Element HTML, w którym ma się pojawić widok.
     * @param autory Lista obiektów Autor do wyświetlenia.
     */
    render(container: HTMLElement, autory: Autor[]): void;
}