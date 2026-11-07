import { Author } from "./models/Author";
import { AuthorService, type AuthorRegistry } from "./services/AuthorService";
import {
    AuthorExporter,
    AuthorFormatterFactory,
    type ExportFormatType
} from "./services/AuthorExporter";
import { HtmlListAuthorFormatter } from "./services/AuthorFormatters";

export class AuthorApp {
    private _authorService: AuthorService;
    private _container: HTMLElement;
    // Zadanie 11: Przechowywanie autorów w strukturze Record zamiast zwyk³ej tablicy
    private _authorsRecord: AuthorRegistry = {};
    private _exporter: AuthorExporter;
    private _contentContainer!: HTMLElement;

    constructor(authorService: AuthorService, container: HTMLElement) {
        this._authorService = authorService;
        this._container = container;
        // Domyœlna strategia: lista punktowana
        this._exporter = new AuthorExporter(new HtmlListAuthorFormatter());
    }

    // Zwracanie tablicy autorów z wartoœci rekordu (na potrzeby eksportera)
    // Zmiana typu zwracanego przez getter authors:
    get authors(): readonly Readonly<Author>[] {
        return Object.keys(this._authorsRecord).map(
            (id) => this._authorsRecord[Number(id)]
        );
    }

    // Zadanie 11: Dostêp do pe³nego rekordu Record<number, Author>
    get authorsRecord(): AuthorRegistry {
        return this._authorsRecord;
    }

    // Zadanie 11: Wyszukiwanie O(1) po kluczu ID
    public getAuthorById(id: number): Author | undefined {
        return this._authorsRecord[id];
    }

    public async init(): Promise<void> {
        try {
            this._container.innerHTML = "<p>Pobieranie listy autorów...</p>";
            // Zadanie 11: Pobieramy dane jako Record<number, Author>
            this._authorsRecord = await this._authorService.fetchAuthorsRecord();

            this._renderUI();
            this._updateContent();
        } catch (error) {
            this._container.innerHTML = `<p style="color: red;">B³¹d: ${(error as Error).message}</p>`;
            console.error("B³¹d podczas inicjalizacji AuthorApp:", (error as Error).message);
        }
    }

    private _renderUI(): void {
        this._container.innerHTML = "";

        // Pasek kontrolny z wyborem formatu
        const controlPanel = document.createElement("div");
        controlPanel.style.display = "flex";
        controlPanel.style.alignItems = "center";
        controlPanel.style.gap = "10px";
        controlPanel.style.margin = "15px 0";

        const label = document.createElement("label");
        label.htmlFor = "format-select";
        label.textContent = "Wybierz formê wyœwietlania:";
        label.style.fontWeight = "bold";

        // Lista rozwijana SELECT
        const select = document.createElement("select");
        select.id = "format-select";
        select.style.padding = "6px 10px";
        select.style.fontSize = "14px";
        select.style.cursor = "pointer";

        // Opcje wyboru zgodne z Zadaniem 7
        const options: { value: ExportFormatType; label: string }[] = [
            { value: "list", label: "Lista punktowana (UL)" },
            { value: "ordered-list", label: "Lista numerowana (OL)" },
            { value: "table", label: "Tabela" }
        ];

        options.forEach((opt) => {
            const option = document.createElement("option");
            option.value = opt.value;
            option.textContent = opt.label;
            select.appendChild(option);
        });

        // Reakcja na zmianê wyboru w liœcie rozwijanej
        select.addEventListener("change", (e: Event) => {
            const target = e.target as HTMLSelectElement;
            const selectedFormat = target.value as ExportFormatType;
            this.setViewFormat(selectedFormat);
        });

        controlPanel.appendChild(label);
        controlPanel.appendChild(select);

        // Kontener na sformatowan¹ treœæ
        this._contentContainer = document.createElement("div");
        this._contentContainer.className = "authors-content";

        this._container.appendChild(controlPanel);
        this._container.appendChild(this._contentContainer);
    }

    public setViewFormat(format: ExportFormatType): void {
        // Podmiana strategii za pomoc¹ fabryki
        const newFormatter = AuthorFormatterFactory.create(format);
        this._exporter.setFormatter(newFormatter);
        this._updateContent();
    }

    private _updateContent(): void {
        if (this._contentContainer) {
            // Wykorzystujemy getter this.authors, który pobiera wartoœci z _authorsRecord
            this._contentContainer.innerHTML = this._exporter.export(this.authors);
        }
    }
    // ==========================================
    // ZADANIE 13: BEZPIECZNE WYSZUKIWANIE AUTORA PO ID
    // ==========================================
    public findAuthorById(id: number): Author | undefined {
        // Odczyt z rekordu w czasie O(1); jeœli klucz nie istnieje, zwraca undefined
        return this._authorsRecord[id];
    }
}