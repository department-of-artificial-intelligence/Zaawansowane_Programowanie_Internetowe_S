import { fromEvent } from 'rxjs';
import { map, scan } from 'rxjs/operators';
export class App {
    constructor(button1, button2, button3, input1, input2, paragraph) {
        this.button3 = button3;
        this.button1 = button1;
        this.button2 = button2;
        this.input1 = input1;
        this.input2 = input2;
    }
    Init() {
        this.observableB1 = fromEvent(this.button1, "click")
            .pipe(map(click => 1), scan((acc, click) => acc + 1, 0));
        this.observableB2 = fromEvent(this.button2, "click")
            .subscribe(x => {
            this.input1.value = x.toString();
        });
        this.observableB3 = fromEvent(this.button3, "click")
            .subscribe(x => {
            this.observableB2.unsubscribe();
        });
    }
    Destroy() {
        this.observableB3.unsubscribe();
        this.observableB1.unsubscribe();
        this.observableB2.unsubscribe();
    }
}
