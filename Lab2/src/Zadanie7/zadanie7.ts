import { fromEvent } from "rxjs";
import { tap } from "rxjs/operators";

export class App
{
    private _button:HTMLButtonElement;
    private _p:HTMLParagraphElement;
    private _stan:number = 0;

    constructor(button:HTMLButtonElement, p:HTMLParagraphElement)
    {
        this._button = button;
        this._p = p;
    }

    public init()
    {
        this._p.innerHTML = this._stan.toString();
        fromEvent(this._button, "click")
            .pipe(tap(_=>this._stan++))
            .subscribe(_=>this._p.innerHTML = this._stan.toString());
    }
}