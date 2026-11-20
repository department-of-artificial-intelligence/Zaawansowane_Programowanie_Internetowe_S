import { Autor } from "../classes/autor";
import type { IAutorFormatter } from "../interfaces/IAutorData";
import type { AutorViewMode } from "../types";

export class AutorContainer{
    private _autors: Record<number, Autor> = {};
    private _autorFormatter: IAutorFormatter;
    private _viewMode: AutorViewMode;

    get viewMode(): AutorViewMode{
        return this._viewMode;
    }
    set viewMode(value: AutorViewMode){
        this._viewMode = value;
        this._viewSelector.value = value;
        this._render();
    }

    get autors(): Record<number, Autor>{
        return this._autors;
    }

    private _wrapper: HTMLDivElement;
    private _viewSelector: HTMLSelectElement;
    private _viewContainer: HTMLDivElement;


    constructor(autorFormatter: IAutorFormatter, initialMode: AutorViewMode = "bullet"){
        this._autorFormatter = autorFormatter;
        this._viewMode = initialMode;

        this._wrapper = document.createElement("div");
        this._viewContainer = document.createElement("div");
        this._viewSelector = document.createElement("select");
        const options: Array<{value: AutorViewMode; label: string}> = [
            {value: "bullet", label: "Bullet list"},
            {value: "numbered", label: "Numbered list"},
            {value: "table", label: "Table"}
        ];

        for(const opt of options){
            const el = document.createElement("option");
            el.value = opt.value;
            el.textContent = opt.label;
            this._viewSelector.appendChild(el);
        }

        this._viewSelector.value = this._viewMode;
        this._viewSelector.addEventListener("change", () => {
            this.viewMode = this._viewSelector.value as AutorViewMode;
        });

        this._wrapper.append(this._viewSelector, this._viewContainer);
    }

    private _render(): void{
        this._viewContainer.replaceChildren();
        if(this._viewMode === "bullet" || this._viewMode === "numbered"){
            const list = document.createElement(this._viewMode === "bullet" ? "ul" : "ol");
            for(const autor of Object.values(this._autors)){
                const li = document.createElement("li");
                li.innerHTML = this._autorFormatter.format(autor);
                list.appendChild(li);
            }
            this._viewContainer.appendChild(list);
            return;
        }

        const table = document.createElement("table");
        const tbody = document.createElement("tbody");
        for(const autor of Object.values(this._autors)){
            const tr = document.createElement("tr");
            const td = document.createElement("td");
            td.innerHTML = this._autorFormatter.format(autor);
            tr.appendChild(td);
            tbody.appendChild(tr);
        }
        table.appendChild(tbody);
        this._viewContainer.appendChild(table);
    }
    
    public async load(): Promise<void> {
        const response = await fetch("/dane.json");
        if(!response.ok)
            throw new Error(`Failed to fetch data ! Status: ${response.status} ${response.statusText}`);

        const raw = await response.json();
        raw.forEach(a => this._autors[a.id] = new Autor(a.imie, a.nazwisko, a.email, a.id));
        this._render();
    }

    public findById(id: number): Autor | undefined {
        return this._autors[id];
    }

    public mount(container: HTMLElement): void{
        container.append(this._wrapper);
    }
}