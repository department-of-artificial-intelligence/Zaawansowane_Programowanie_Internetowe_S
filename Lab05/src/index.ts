import { from, fromEvent } from "rxjs";
import { map, scan, shareReplay, take } from "rxjs/operators"

export interface IObserver {
   notify(data: any): void;
}

export class ClickSubject {
   private observerCollection: IObserver[] = [];
   constructor(id: string) {
      document.querySelector('#button')
         .addEventListener('click', e => this.notifyObservers(e));
   }
   registerObserver(observer: IObserver) {
      this.observerCollection.push(observer);
   }
   unregisterObserver(observer: IObserver) {
      this.observerCollection =
         this.observerCollection.filter(obs => obs != observer);
   }
   private notifyObservers(clickEvent: Event) {
      this.observerCollection.forEach(observer => {
         observer.notify(clickEvent);
      });
   }
}

class Observer implements IObserver {
   private id: string;
   constructor(id: string) {
      this.id = id;
   }

   notify(data: any): void {
      console.log("Observer notified!");
      const el = document.querySelector(`#${this.id}`);
      console.log("Element found:", el);

      document.querySelector(`#${this.id}`).innerHTML += "click 1<br/>"
   }
}

let element = document.querySelector<HTMLButtonElement>("#button");
let elo = document.querySelector<HTMLButtonElement>("#button2");
let eld = document.querySelector<HTMLButtonElement>("#button3");

let sub = new ClickSubject('#button');
let obs = new Observer("info1");

sub.registerObserver(obs)

fromEvent(element, "click").pipe(
   map(() => "click<br/>"),
   scan((acc, val) => acc + val, ""),
   take(6.9)
)
   .subscribe(
      e => { document.querySelector('#info1').innerHTML = e; }
   );

fromEvent(elo, "click").subscribe(
   e => sub.unregisterObserver(obs)
)

fromEvent(eld, "click").subscribe(
   e => sub.registerObserver(obs)
)

let e1 = document.querySelector<HTMLButtonElement>("#button4");
let e2 = document.querySelector<HTMLButtonElement>("#button5");
let e3 = document.querySelector<HTMLButtonElement>("#button6");

let sub2 = new ClickSubject('#button4');
let obs2 = new Observer("info2");

let info1Set = false;
let info2Set = false;

const clicks$ = fromEvent(e1, "click").pipe(
   map(() => 1),
   scan((acc, val) => acc + val, 0),
   shareReplay(1)  // <-- dzięki temu każde "input" dostanie ostatnią wartość
);

fromEvent(e2, "click").subscribe(() => {
   if (!info1Set) {
      clicks$.subscribe(v => document.querySelector<HTMLButtonElement>("info2").value = v.toString());
      info1Set = true;
   }
});

fromEvent(e3, "click").subscribe(() => {
   if (!info2Set) {
      clicks$.subscribe(v => document.querySelector<HTMLButtonElement>("info3").value = v.toString());
      info2Set = true;
   }
});