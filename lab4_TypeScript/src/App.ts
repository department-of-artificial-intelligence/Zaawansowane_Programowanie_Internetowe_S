import { Autor } from "./Autor";
import { AuthorService } from "./AuthorService";
// Importujemy nasze nowe strategie
import { IAuthorView } from "./IAuthorView";
import { ListView } from "./ListView";
import { NumberedListView } from "./NumberedListView";
import { TableView } from "./TableView";

export class App {
    private authorService: AuthorService;
    private appContainer: HTMLElement;
    private viewSelectElement: HTMLSelectElement;

    private autory: Autor[] = []; // Przechowujemy pobranych autorów

    // Zbiór dostępnych strategii
    private viewStrategies: Map<string, IAuthorView>;
    private currentView: IAuthorView;

    constructor(containerSelector: string, selectSelector: string) {
        this.authorService = new AuthorService("dane.json");

        // Inicjalizacja elementów DOM
        this.appContainer = document.querySelector(containerSelector)!;
        this.viewSelectElement = document.querySelector(selectSelector)!;

        if (!this.appContainer || !this.viewSelectElement) {
            throw new Error("Nie można znaleźć kontenera aplikacji lub listy wyboru widoku.");
        }

        // Inicjalizacja strategii
        this.viewStrategies = new Map<string, IAuthorView>();
        this.viewStrategies.set("list", new ListView());
        this.viewStrategies.set("numbered-list", new NumberedListView());
        this.viewStrategies.set("table", new TableView());

        // Ustawienie domyślnej strategii
        this.currentView = this.viewStrategies.get("list")!;
    }

    public async start(): Promise<void> {
        console.log("Aplikacja startuje...");
        this.appContainer.innerHTML = "<p>Ładowanie autorów...</p>";

        try {
            // 1. Pobieramy dane i zapisujemy je w klasie
            this.autory = await this.authorService.getAuthors();

            // 2. Podpinamy listener do listy rozwijanej
            this.viewSelectElement.addEventListener("change", this.onViewChange);

            // 3. Renderujemy widok po raz pierwszy (domyślną strategią)
            this.render();

        } catch (error: any) {
            console.error("Błąd startu aplikacji:", error.message);
            this.displayError(error.message);
        }
    }

    /**
     * Prywatna metoda wywoływana przy zmianie <select>
     */
    private onViewChange = (event: Event): void => {
        // 'event.target' to nasz <select>
        const selectedValue = (event.target as HTMLSelectElement).value;

        // Pobieramy nową strategię z naszej mapy
        const newStrategy = this.viewStrategies.get(selectedValue);

        if (newStrategy) {
            this.currentView = newStrategy;
            // Natychmiast przerysowujemy widok z nową strategią
            this.render();
        } else {
            console.error(`Nie znaleziono strategii dla wartości: ${selectedValue}`);
        }
    }

    /**
     * Główna metoda renderująca.
     * Deleguje renderowanie do aktualnie wybranej strategii.
     */
    private render(): void {
        console.log(`Renderowanie widoku za pomocą: ${this.currentView.constructor.name}`);

        if (this.autory.length === 0) {
            this.appContainer.innerHTML = "<p>Nie znaleziono żadnych autorów.</p>";
            return;
        }

        // Używamy aktualnej strategii do renderowania
        this.currentView.render(this.appContainer, this.autory);
    }

    private displayError(message: string): void {
        this.appContainer.innerHTML = `<p style="color: red; font-weight: bold;">Błąd: ${message}</p>`;
    }
}