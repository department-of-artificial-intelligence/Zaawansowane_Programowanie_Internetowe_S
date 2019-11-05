import { fromEvent, Subscription, Observable } from "rxjs";
import { tap, map, scan, take } from "rxjs/operators";

export class App
{
    private _button:HTMLButtonElement;
    private _p:HTMLParagraphElement;
    private _wlacz:HTMLButtonElement;
    private _wylacz:HTMLButtonElement;
    private _sub:Subscription;
    private _obs:Observable<number>;

    constructor(button:HTMLButtonElement, 
                p:HTMLParagraphElement,
                wlacz:HTMLButtonElement,
                wylacz:HTMLButtonElement)
    {
        this._button = button;
        this._p = p;
        this._wlacz = wlacz;
        this._wylacz = wylacz;
    }

    public init()
    {
        this._p.innerHTML = "0";
        this._obs =  fromEvent(this._button, "click")
                        .pipe(
                            map(_=>1),
                            scan((a,x)=>a+x)
                            );
        fromEvent(this._wlacz, "click").subscribe(_=>this.wlaczClick());
        fromEvent(this._wylacz, "click").subscribe(_=>this.wylaczClick());
    }

    private wlaczClick() {
        this._sub = this._obs.subscribe(v=>this._p.innerHTML = v.toString());
    }

    private wylaczClick() {
        this._sub.unsubscribe();
    }

}