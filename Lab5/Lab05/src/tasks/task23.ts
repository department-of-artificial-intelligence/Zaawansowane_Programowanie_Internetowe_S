import { fromEvent } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { ajax } from "rxjs/ajax";
import { addButton, addInput } from "../dom";

export function task23(parent: HTMLElement): void {
    const form = document.createElement("form");
    ["firstName", "lastName", "email"].forEach(name => {
        addInput(form, "text").name = name;
    });
    addButton(form, "Send");
    parent.appendChild(form);
    fromEvent(form, "submit")
        .pipe(
            tap(event => event.preventDefault()),
            switchMap(() => ajax({
                url: "/proceed",
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: Object.fromEntries(new FormData(form))
            })))
        .subscribe(response => console.log(response.response));
}
