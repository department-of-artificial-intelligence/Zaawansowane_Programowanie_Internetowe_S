import { fromEvent } from "rxjs";
import { map, scan } from "rxjs/operators";

class ClickCounterApp {            
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  start(): void {
    fromEvent(this.button, "click")        
      .pipe(
        scan((acc: number, _) => acc + 1, 0),
        map(count => `Kliknięcia: ${count}`)      
      )
      .subscribe(text => {                   
        this.paragraph.textContent = text;
      });
  }
}

new ClickCounterApp("btn", "licznik").start();