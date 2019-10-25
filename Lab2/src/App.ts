import { fromEvent, Observable } from 'rxjs';
import { tap, map, take } from 'rxjs/operators';

export class App {
    //  clickCount: number;
    button1: HTMLButtonElement;
    button2: HTMLButtonElement;
    button: HTMLButtonElement;
    paragraph: HTMLParagraphElement;
    observableB1: any;
    observableB2: any;
    observableB: any
    constructor(button1: HTMLButtonElement, button2: HTMLButtonElement, button: HTMLButtonElement, paragraph: HTMLParagraphElement) {
        this.button = button;
        this.button1 = button1;
        this.button2 = button2;
        this.paragraph = paragraph;
        //  this.clickCount = 0;
        this.paragraph.innerText = "0"//this.clickCount.toString()

    }

    public Init(): void {


        this.observableB1 = fromEvent(this.button1, "click")
            .subscribe(() => {
                this.observableB = fromEvent(this.button, 'click')
                    .pipe(
                        take(5)
                    ).subscribe(() => {
                        //this.clickCount++;
                        let clickCount = parseInt(this.paragraph.innerText)
                        clickCount++
                        this.paragraph.innerText = clickCount.toString()
                    });
            });
        this.observableB2 = fromEvent(this.button2, "click")
            .subscribe(() => {
                this.observableB.unsubscribe();
            });
    }
    public Destroy() {
        this.observableB.unsubscribe();
        this.observableB1.unsubscribe();
        this.observableB2.unsubscribe();
    }
}