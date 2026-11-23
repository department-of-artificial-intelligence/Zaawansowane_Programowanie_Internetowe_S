import { fromEvent } from "rxjs";
import { map, scan, take } from "rxjs/operators";

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
        take(5),
        scan((acc: number, _) => acc + 1, 0),
        map(count => `Kliknięcia: ${count}`)      
      )
      .subscribe({
        next: text => {                   
        this.paragraph.textContent = text;
        },
        complete: () => {
            this.paragraph.textContent += " (limit osiągnięty)";
            this.button.disabled = true;
        }
      });
  }
}

new ClickCounterApp("btn", "licznik").start();