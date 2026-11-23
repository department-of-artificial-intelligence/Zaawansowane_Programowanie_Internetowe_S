import { fromEvent, map, delay } from "rxjs";

export class DivFollower{
    private _wrapper: HTMLDivElement;
    private _div: HTMLDivElement;

    constructor(moveDelay: number = 0){
        this._wrapper = document.createElement("div");
        this._div = document.createElement("div");
        this._div.style.position = "fixed";
        this._div.style.backgroundColor = "#333333";
        this._div.style.width = "24px";
        this._div.style.height = "24px";
        this._wrapper.appendChild(this._div);

        fromEvent<MouseEvent>(document, "mousemove").pipe(map(e => ({ x: e.clientX, y: e.clientY })), delay(moveDelay))
            .subscribe(pos => { this._div.style.left = pos.x + "px"; this._div.style.top = pos.y + "px"});
        
    }

    public mount(container: HTMLElement): void{
        container.appendChild(this._wrapper);
    }
}
