import { fromEvent } from "rxjs";

export function task19(parent: HTMLElement): void {
    const div = document.createElement("div");
    div.style.position = "fixed";
    div.style.width = "20px";
    div.style.height = "20px";
    div.style.backgroundColor = "red";

    div.style.pointerEvents = "none";
    parent.appendChild(div);

    fromEvent<MouseEvent>(document, "mousemove")
        .subscribe(event => {
            div.style.left = `${event.clientX}px`;
            div.style.top = `${event.clientY}px`;
        });
}