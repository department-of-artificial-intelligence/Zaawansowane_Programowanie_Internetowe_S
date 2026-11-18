import { fromEvent } from "rxjs";
import { delay } from "rxjs/operators";

export function task20(parent: HTMLElement): void {
    const div = document.createElement("div");
    div.style.position = "fixed";
    div.style.width = "20px";
    div.style.height = "20px";
    div.style.backgroundColor = "blue";
    parent.appendChild(div);
    fromEvent<MouseEvent>(document, "mousemove")
        .pipe(delay(100))
        .subscribe(event => {
            div.style.left = `${event.clientX}px`;
            div.style.top = `${event.clientY}px`;
        });
}
