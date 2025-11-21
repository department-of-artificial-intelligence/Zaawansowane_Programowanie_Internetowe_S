import { Autor } from "./Autor";
import { IAuthorFormatter } from "./IAuthorFormatter";

// "Kontekst" - klasa, która używa strategii.
export class AuthorExporter {

    // Prywatne pole przechowujące aktualnie wybraną strategię
    private formatter: IAuthorFormatter;

    // W konstruktorze wymuszamy ustawienie domyślnej strategii
    constructor(domyslnaStrategia: IAuthorFormatter) {
        if (!domyslnaStrategia) {
            throw new Error("Należy podać domyślną strategię formatowania.");
        }
        this.formatter = domyslnaStrategia;
    }

    /**
     * Publiczna metoda pozwalająca na zmianę strategii w locie.
     * @param nowaStrategia Nowy obiekt formatera
     */
    public setStrategy(nowaStrategia: IAuthorFormatter) {
        this.formatter = nowaStrategia;
    }

    /**
     * Główna metoda wykonawcza.
     * Deleguje zadanie formatowania do aktualnie wybranej strategii.
     * @param autory Lista autorów do wyeksportowania
     * @returns Sformatowany string
     */
    public export(autory: Autor[]): string {
        // Kontekst nie wie, JAK formatować.
        // Po prostu wywołuje metodę format() na aktualnym obiekcie strategii.
        return this.formatter.format(autory);
    }
}