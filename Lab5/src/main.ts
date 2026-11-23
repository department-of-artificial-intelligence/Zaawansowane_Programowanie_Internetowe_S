import { fromEvent, Subscription } from "rxjs";
import { map, scan } from "rxjs/operators";

class App {            
  private startButton: HTMLButtonElement;
  private stopButton: HTMLButtonElement;
  private countButton: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;
  private subscription: Subscription | null = null;

  constructor(startId: string, stopId: string, countId: string, paragraphId: string) {
    this.startButton = document.querySelector<HTMLButtonElement>(`#${startId}`)!;
    this.stopButton = document.querySelector<HTMLButtonElement>(`#${stopId}`)!;
    this.countButton = document.querySelector<HTMLButtonElement>(`#${countId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  init(): void {
    fromEvent(this.startButton, "click").subscribe(() => this.startCounting());
    fromEvent(this.stopButton, "click").subscribe(() => this.stopCounting());
  }

  private startCounting(): void {
    if (this.subscription) return;                    
    this.paragraph.textContent = "Liczenie włączone";
    this.subscription = fromEvent(this.countButton, "click")
      .pipe(
        scan((acc: number, _) => acc + 1, 0),
        map(count => `Kliknięcia: ${count}`)
      )
      .subscribe(text => {
        this.paragraph.textContent = text;
      });
  }

  private stopCounting(): void {
    if (!this.subscription) return;                    
    this.subscription.unsubscribe();                   
    this.subscription = null;
    this.paragraph.textContent += " (zatrzymano)";
  }
}

new App("start", "stop", "licz", "licznik").init();