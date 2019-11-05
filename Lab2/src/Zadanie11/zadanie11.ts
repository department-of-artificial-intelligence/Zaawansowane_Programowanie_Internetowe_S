import {fromEvent, Observable, Subscription} from "rxjs";
import {map, scan, share} from "rxjs/operators"

export class App
{
    private _button:HTMLButtonElement;
    private _start:HTMLButtonElement;
    private _stop:HTMLButtonElement;
    private _i1 : HTMLInputElement;
    private _i2 : HTMLInputElement;
    private click$:Observable<number>;
    private _s1 : Subscription;
    private _s2 : Subscription;

    constructor(button:HTMLButtonElement, start:HTMLButtonElement, stop:HTMLButtonElement, i1:HTMLInputElement, i2:HTMLInputElement)
    {
        this._button = button;
        this._start = start;
        this._stop = stop;
        this._i1 = i1;
        this._i2 = i2;
        this._s1 = null;
        this._s2 = null;
    }

    public init()
    {
        this.click$ = fromEvent(this._button, "click")
         .pipe(map(_=>1),
               scan((a,x)=>a+x),
              );

        fromEvent(this._start, "click").subscribe(_=>this.zmien1());
        fromEvent(this._stop, "click").subscribe(_=>this.zmien2());
    }

    private zmien1()
    {
            if(this._s1 == null)
                this._s1 = this.click$.subscribe(w=>this._i1.value = w.toString());
            else
            {
                this._s1.unsubscribe();
                this._s1 = null;
            }      
    }  

    private zmien2()
    {
            if(this._s2 == null)
                this._s2 = this.click$.subscribe(w=>this._i2.value = w.toString());
            else
            {
                this._s2.unsubscribe();
                this._s2 = null;
            }      
    }  
}
