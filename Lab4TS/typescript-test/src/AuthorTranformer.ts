import { Autor } from './author.ts';
import { AutorFormatter, HtmlListFormatter, HtmlTableFormatter} from './AuthorExporter.ts';
import { measureTime } from './measureTime.ts';

interface AutorDTO {
  id?: number;
  imie: string;
  nazwisko: string;
  email: string;
}

type ViewMode = 'ul' | 'ol' | 'table';



export class AutorTransformer {
    private _autorzy: Readonly<Record<number, Readonly<Autor>>> = {}
    private readonly _container: HTMLElement;
    private readonly _outputEl: HTMLDivElement;
    private readonly _selectEl: HTMLSelectElement;
    private readonly _formatters: Record<ViewMode, AutorFormatter> = {
        ul: new HtmlListFormatter(false),
        ol: new HtmlListFormatter(true),
        table: new HtmlTableFormatter(),
    };

    constructor(container: HTMLElement, private readonly _dataUrl: string = './dane.json') 
    {
        this._container = container;
        this._selectEl = this.createDropdown();
        this._outputEl = document.createElement('div');

        this._container.append(this._selectEl, this._outputEl);
        this._selectEl.addEventListener('change', () => this.render());
    }

    public async init(): Promise<void> {
        this._autorzy = await this.fetchAutorzy();
        this.render();
    }

    private async fetchAutorzy(): Promise<Readonly<Record<number, Readonly<Autor>>>> {
        const response = await fetch(this._dataUrl);
        if (!response.ok) {
            throw new Error(`Błąd pobierania danych: HTTP ${response.status}`);
        }

        const rawData: AutorDTO[] = await response.json();

        return rawData.reduce<Record<number, Readonly<Autor>>>((acc, item) => {
            const autor = new Autor(item.imie, item.nazwisko, item.email);
            acc[autor.id] = autor;
            return acc;
        }, {});
    }

    public znajdzAutoraPoId(id: number): Readonly<Autor> | undefined {
        return this._autorzy[id];
    }

    public wyswietlAutoraPoId(id: number): string {
        const autor = this.znajdzAutoraPoId(id);
        const komunikat = autor?.pelneImie ?? 'Nie znaleziono autora';
        console.log(komunikat);
        return komunikat;
    }

    private createDropdown(): HTMLSelectElement {
        const select = document.createElement('select');
        const options: { value: ViewMode; label: string }[] = [
            { value: 'ul', label: 'Lista punktowana' },
            { value: 'ol', label: 'Lista numerowana' },
            { value: 'table', label: 'Tabela' },
        ];

        for (const opt of options) {
            const optionEl = document.createElement('option');
            optionEl.value = opt.value;
            optionEl.textContent = opt.label;
            select.appendChild(optionEl);
        }

        return select;
    }

    @measureTime
    public render(): void {
        const listaTylkoDoOdczytu: readonly Readonly<Autor>[] = Object.values(this._autorzy);
        const mode = this._selectEl.value as ViewMode;
        const formatter = this._formatters[mode];
        this._outputEl.innerHTML = formatter.format(listaTylkoDoOdczytu);
  }
}

// Entry point on app startup:
// const appRoot = document.getElementById('app');
// if (appRoot) {
//   const app = new AutorTransformer(appRoot, './dane.json');
//   app.init();
// }