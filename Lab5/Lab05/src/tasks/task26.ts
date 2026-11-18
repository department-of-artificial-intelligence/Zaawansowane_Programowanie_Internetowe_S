import { fromEvent } from "rxjs";
import { debounceTime, distinctUntilChanged, map, switchMap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";
import { addInput, addParagraph } from "../dom";

export function task26(parent: HTMLElement): void {
    const input = addInput(parent, "search");
    const paragraph = addParagraph(parent);
    fromEvent(input, "input")
        .pipe(
            map(() => input.value),
            debounceTime(500),
            distinctUntilChanged(),
            switchMap(() => ajax("/dane.json")))
        .subscribe(response => paragraph.textContent = JSON.stringify(response.response));
}
