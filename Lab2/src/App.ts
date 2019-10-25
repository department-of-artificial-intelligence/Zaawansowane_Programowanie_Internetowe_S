import { fromEvent, Observable } from 'rxjs';
import { tap, map, take, scan, share } from 'rxjs/operators';

export class App {
    button1: HTMLButtonElement;
    button2: HTMLButtonElement;
    button3: HTMLButtonElement;
    observableB1: any;
    observableB2: any;
    observableB3: any
    input1: HTMLInputElement;
    input2: HTMLInputElement;
    constructor(button1: HTMLButtonElement, button2: HTMLButtonElement, button3: HTMLButtonElement,
        input1: HTMLInputElement, input2: HTMLInputElement, paragraph: HTMLParagraphElement) {
        this.button3 = button3;
        this.button1 = button1;
        this.button2 = button2;
        this.input1 = input1;
        this.input2 = input2;
    }

    public clickerFunction (input:HTMLInputElement){
        return fromEvent(this.button1, "click")
                    .pipe(
                        map(click => 1),
                        scan((acc, click) => acc + 1, 0),
                        share()
                    ).subscribe(x => {
                        input.value = x.toString();
                    });
    }

    public Init(): void {        
        this.observableB2 = fromEvent(this.button2, "click")
            .subscribe(() => {
                if (this.observableB1 == null) {
                    this.observableB1 = this.clickerFunction(this.input1);
                }
                else {
                   // this.observableB1.unsubscribe(); 
                    this.observableB1 = this.clickerFunction(this.input2);
                }
            });
        this.observableB3 = fromEvent(this.button3, "click")
            .subscribe(x => {
                this.observableB1.unsubscribe();
            });
    }
    public Destroy() {
        this.observableB3.unsubscribe();
        this.observableB1.unsubscribe();
        this.observableB2.unsubscribe();
    }
}