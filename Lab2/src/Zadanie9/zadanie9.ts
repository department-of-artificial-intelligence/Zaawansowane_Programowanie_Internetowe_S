import { fromEvent } from "rxjs";
import { tap, map, scan, take } from "rxjs/operators";

export class App
{
    private _button:HTMLButtonElement;
    private _p:HTMLParagraphElement;

    constructor(button:HTMLButtonElement, p:HTMLParagraphElement)
    {
        this._button = button;
        this._p = p;
    }

    public init()
    {
        this._p.innerHTML = "0";
        fromEvent(this._button, "click")
            .pipe(take(5),
                  map(_=>1),
                  scan((a,x)=>a+x)
                 )
            .subscribe(x=>this._p.innerHTML = x.toString());
    }
}