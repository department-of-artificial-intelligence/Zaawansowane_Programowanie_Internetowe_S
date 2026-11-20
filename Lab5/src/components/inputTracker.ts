import { combineLatest, distinctUntilChanged, fromEvent, map, startWith } from "rxjs";

export class InputTracker {
    private _paragraph: HTMLParagraphElement;
    private _input: HTMLInputElement;
    private _lengthInput: HTMLInputElement;
    private _wrapper: HTMLDivElement;

    constructor() {
        this._wrapper = document.createElement("div");

        this._paragraph = document.createElement("p");

        this._input = document.createElement("input");
        this._input.type = "text";

        this._lengthInput = document.createElement("input");
        this._lengthInput.type = "number";
        this._lengthInput.value = "0";

        const parseThreshold = (raw: string): number => {
            const trimmed = raw.trim();
            return trimmed === "" ? NaN : Number(trimmed);
        };

        const inputSource$ = fromEvent(this._input, "input").pipe(
            map(() => this._input.value),
            startWith(this._input.value)
        );

        const lengthInputSource$ = fromEvent(this._lengthInput, "input").pipe(
            map(() => parseThreshold(this._lengthInput.value)),
            startWith(parseThreshold(this._lengthInput.value))
        );

        combineLatest([inputSource$, lengthInputSource$])
            .pipe(
                map(([text, minLength]) => {
                    if (Number.isNaN(minLength)) {
                        return text;
                    }
                    if (text.length <= minLength) {
                        return "";
                    }
                    return text.length > minLength ? text : this._paragraph.innerText;
                }),
                distinctUntilChanged()
            )
            .subscribe((nextText) => {
                this._paragraph.innerText = nextText;
            });

        this._wrapper.append(
            this._input,
            this._lengthInput,
            this._paragraph
        );
    }

    public mount(container: HTMLElement): void {
        container.appendChild(this._wrapper);
    }
}