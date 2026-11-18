import { ajax } from "rxjs/ajax";

export function task21(): void {
    ajax("dane.json")
        .subscribe(response => console.log(response.response));
}
