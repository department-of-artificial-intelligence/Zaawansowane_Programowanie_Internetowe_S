import { fromEvent } from 'rxjs';
import { tap } from 'rxjs/operators';

export class App {
    clickCount: number;
    button: HTMLButtonElement;
    paragraph: HTMLParagraphElement
    constructor(button: HTMLButtonElement, paragraph: HTMLParagraphElement) {
        this.button = button;
        this.paragraph = paragraph;
        this.clickCount = 0;
        this.paragraph.innerText = this.clickCount.toString()
    }

    public Init(): void {
        fromEvent(this.button, 'click')
            .pipe(
                tap(c => {
                    this.clickCount++;
                    this.paragraph.innerText = this.clickCount.toString()
                })
            ).subscribe();
    }
}