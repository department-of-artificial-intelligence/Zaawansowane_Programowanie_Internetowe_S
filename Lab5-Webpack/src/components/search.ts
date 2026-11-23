import { debounceTime, distinctUntilChanged, fromEvent, map, switchMap } from "rxjs";
import { ajax } from "rxjs/ajax";
import { Component } from "./component";

interface SearchResultItem {
    id?: number | string;
    name?: string;
    surname?: string;
    email?: string;
    [key: string]: unknown;
}

export class Search extends Component {
    private _input: HTMLInputElement;
    private _resultsContainer: HTMLDivElement;

    constructor() {
        super();

        this._input = document.createElement("input");
        this._input.type = "text";
        this._input.placeholder = "Search...";

        this._resultsContainer = document.createElement("div");

        this.appendChild(this._input);
        this.appendChild(this._resultsContainer);

        fromEvent(this._input, "input")
            .pipe(
                map(() => this._input.value.trim()),
                debounceTime(500),
                distinctUntilChanged(),
                switchMap(() => ajax("http://localhost:9000/dane.json"))
            )
            .subscribe({
                next: (data) => this._renderResults(data),
                error: (err) => {
                    console.error("Search error:", err);
                    this._resultsContainer.innerText = "Error loading results.";
                },
            });
    }

    private _renderResults(data: SearchResultItem[] | unknown): void {
        this._resultsContainer.replaceChildren();

        if (Array.isArray(data)) {
            if (data.length === 0) {
                this._resultsContainer.innerText = "No matching results.";
                return;
            }

            const list = document.createElement("ul");
            for (const item of data) {
                const li = document.createElement("li");
                const label =
                    [item.name, item.surname].filter(Boolean).join(" ") ||
                    item.title ||
                    item.email ||
                    JSON.stringify(item);
                li.innerText = String(label);
                list.appendChild(li);
            }
            this._resultsContainer.appendChild(list);
            return;
        }

        this._resultsContainer.innerText = JSON.stringify(data, null, 2);
    }
}