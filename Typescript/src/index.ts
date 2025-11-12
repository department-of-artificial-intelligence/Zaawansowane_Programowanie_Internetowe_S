import { map, scan, take, shareReplay } from 'rxjs/operators';
import { fromEvent, Subscription } from 'rxjs';

export interface IObserver {
    liczKlik(data: any): void;
}

export class ClickSubject {
    private observerCollection: IObserver[] = [];

    constructor(id: string) {
        const btn = document.createElement('button');
        btn.id = `${id}`;
        btn.textContent = 'Kliknij mnie';
        document.body.appendChild(btn);

        //btn.addEventListener('click', e => this.liczKlik(e));
        fromEvent(btn, 'click')
            .pipe(
                map(() => 1),
                scan((acc, val) => acc + val, 0),
                take(5)
            )
            .subscribe({
                next: count => {
                    console.log("Klik: " + count);
                },
                complete: () => {
                    btn.disabled = true;
                    console.log("Koniec")
                }
            })
    }
    registerObserver(observer: IObserver) {
        this.observerCollection.push(observer);
    }
    unregisterObserver(observer: IObserver) {
        this.observerCollection =
            this.observerCollection.filter(obs => obs != observer);
    }
    private liczKlik(clickEvent: Event) {
        this.observerCollection.forEach(observer => {
            observer.liczKlik(clickEvent);
        });
    }
}


export class przyciski {
    private counterSub: Subscription | null;
    constructor() {
        this.counterSub = null;
        const startBtn = document.createElement('button');
        startBtn.textContent = 'Start';
        document.body.appendChild(startBtn);

        const stopBtn = document.createElement('button');
        stopBtn.textContent = 'Stop';
        document.body.appendChild(stopBtn);

        const counterBtn = document.createElement('button');
        counterBtn.id = "counter";
        counterBtn.textContent = 'Kliknij mnie';
        document.body.appendChild(counterBtn);

        const counterClicks$ = fromEvent(counterBtn, 'click').pipe(
            map(() => 1),
            scan((acc, val) => acc + val, 0)
        );
        fromEvent(startBtn, 'click').subscribe(() => {
            if (!this.counterSub || this.counterSub.closed) {
                this.counterSub = counterClicks$.subscribe(count => {
                    console.log('Kliknięcia:', count);
                });
            }
        });
        fromEvent(stopBtn, 'click').subscribe(() => {
            if (this.counterSub) {
                this.counterSub.unsubscribe();
                console.log('Subskrypcja zatrzymana');
            }
        });
    }
}

export class Przyciski2 {
    private counterSub: Subscription | null;
    private display1Blocked = false;
    private display2Blocked = false;

    constructor() {
        this.counterSub = null;

        const counterBtn = document.createElement('button');
        counterBtn.textContent = 'Kliknij mnie';
        document.body.appendChild(counterBtn);

        const showBtn = document.createElement('button');
        showBtn.textContent = 'Pokaż liczbę';
        document.body.appendChild(showBtn);

        const dummyBtn = document.createElement('button');
        dummyBtn.textContent = 'Przycisk 3';
        document.body.appendChild(dummyBtn);

        const input1 = document.createElement('input');
        input1.placeholder = 'Licznik 1';
        document.body.appendChild(input1);

        const input2 = document.createElement('input');
        input2.placeholder = 'Licznik 2';
        document.body.appendChild(input2);

        const counterClicks$ = fromEvent(counterBtn, 'click').pipe(
            map(() => 1),
            scan((acc, val) => acc + val, 0),
            shareReplay(1) 
        );

        fromEvent(showBtn, 'click').subscribe(() => {
            if (!this.display1Blocked) {
                this.display1Blocked = true;
                counterClicks$.subscribe(count => {
                    input1.value = count.toString();
                });
            } else if (!this.display2Blocked) {
                this.display2Blocked = true;
                counterClicks$.subscribe(count => {
                    input2.value = count.toString();
                });
            }
        });

        fromEvent(dummyBtn, 'click').subscribe(() => {
            console.log('Przycisk 3 kliknięty');
        });
    }
}


class Observer implements IObserver {
    private id: string;
    private klik: number;

    constructor(id: string) {
        this.id = id;
        const info = document.createElement('p');
        info.id = `counter_${id}`;
        this.klik = 0;
        info.innerHTML = 'Klikniecia: ' + this.klik;
        document.body.appendChild(info);
    }
    liczKlik(data: any): void {
        const element = document.querySelector(`#counter_${this.id}`);
        if (element) {
            this.klik++;
            element.innerHTML = 'Klikniecia: ' + this.klik;
        }
    }
}

// Zadanie 01 
const tab1: number[] = [1, 2, 3, 4, 5];
const sum: number = tab1.reduce((prev, current) => prev + current);
const sumEven: number = (tab1.filter(x => x % 2 == 0)).reduce((prev, current) => prev + current);

function greaterThan(value: number): number {
    return (tab1.filter(x => x > value)).reduce((prev, current) => prev + current, 0);
}



function hello() {
    /*console.log("Suma " + sum);
    console.log("Suma parzystych " + sumEven);
    var valueStr = prompt("Podaj liczbe");
    if (valueStr === null) {
        alert("Nieprawidlowa wartosc");
    } else {
        let value: number = parseInt(valueStr);
        if (Number.isNaN(value)) {
            alert("Nieprawidlowa wartosc");
            return;
        }
        console.log("Suma powyzej wpisanej liczby " + greaterThan(value));
    }*/
}
hello();



function reaktywne() {
    //let subject = new ClickSubject("button");
    //let observer1 = new Observer("info1");
    //subject.registerObserver(observer1);
    //let threeBtns = new przyciski();
    new Przyciski2();
}

reaktywne();