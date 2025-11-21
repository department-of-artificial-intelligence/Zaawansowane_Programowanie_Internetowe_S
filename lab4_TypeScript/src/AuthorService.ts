import { Autor } from "./Autor";

// Definiujemy interfejs dla "surowych" danych z JSON.
// To dobra praktyka w TypeScript.
interface IRawAutorData {
    imie: string;
    nazwisko: string;
    email: string;
}

/**
 * Serwis odpowiedzialny za pobieranie i transformację danych Autorów.
 */
export class AuthorService {

    private dataUrl: string;

    constructor(dataUrl: string) {
        this.dataUrl = dataUrl;
    }

    /**
     * Asynchronicznie pobiera listę autorów z serwera.
     * @returns Obietnica (Promise) zwracająca tablicę instancji klasy Autor.
     */
    public async getAuthors(): Promise<Autor[]> {

        // 1. Używamy 'fetch' do pobrania danych
        const response = await fetch(this.dataUrl);

        // 2. Sprawdzamy, czy żądanie się powiodło
        if (!response.ok) {
            throw new Error(`Nie udało się pobrać danych z ${this.dataUrl}. Status: ${response.status}`);
        }

        // 3. Parsujemy odpowiedź jako JSON
        const rawData: IRawAutorData[] = await response.json();

        // 4. Transformujemy surowe dane (obiekty JSON) na instancje klasy Autor
        // Jest to kluczowy moment "zamiany na listę obiektów klasy Autor"
        const autory: Autor[] = rawData
            .map(data => {
                try {
                    // Używamy konstruktora klasy Autor, który ma walidację (z Zad. 2)
                    return new Autor(data.imie, data.nazwisko, data.email);
                } catch (error: any) {
                    console.warn(`Pominięto autora z powodu błędu walidacji: ${error.message}`, data);
                    return null; // Zwracamy null dla niepoprawnych danych
                }
            })
            // Filtrujemy, aby usunąć te pozycje, które zwróciły null
            .filter(autor => autor !== null) as Autor[];

        return autory;
    }
}