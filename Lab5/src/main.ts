import { fromEvent } from "rxjs";
import { tap } from "rxjs/operators";

class ClickCounterApp {
  private count: number = 0;              
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.querySelector<HTMLButtonElement>(`#${buttonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  start(): void {
    fromEvent(this.button, "click")        
      .pipe(
        tap(() => this.count++)            
      )
      .subscribe(() => {                   
        this.paragraph.textContent = `Kliknięcia: ${this.count}`;
      });
  }
}

const app = new ClickCounterApp("btn", "licznik");
app.start();