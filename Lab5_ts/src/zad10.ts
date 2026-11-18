// Zadanie 10 - przycisk 1 włącza zliczanie, przycisk 2 je wyłącza,
// zliczamy kliknięcia przycisku 3 (subscribe / unsubscribe)
import { fromEvent, Subscription } from "rxjs";
import { map, scan } from "rxjs/operators";

export class Zad10 {
  private enableButton: HTMLButtonElement;
  private disableButton: HTMLButtonElement;
  private countButton: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;
  private subscription?: Subscription;

  constructor(enableButtonId: string, disableButtonId: string,
              countButtonId: string, paragraphId: string) {
    this.enableButton = document.querySelector<HTMLButtonElement>(`#${enableButtonId}`)!;
    this.disableButton = document.querySelector<HTMLButtonElement>(`#${disableButtonId}`)!;
    this.countButton = document.querySelector<HTMLButtonElement>(`#${countButtonId}`)!;
    this.paragraph = document.querySelector<HTMLParagraphElement>(`#${paragraphId}`)!;
  }

  public start(): void {
    fromEvent(this.enableButton, "click").subscribe(() => this.enable());
    fromEvent(this.disableButton, "click").subscribe(() => this.disable());
  }

  private enable(): void {
    if (!this.subscription) {
      this.paragraph.innerHTML = "Zliczanie włączone";
      this.subscription = fromEvent(this.countButton, "click")
        .pipe(
          scan((count, _) => count + 1, 0),
          map(count => `Liczba kliknięć: ${count}`)
        )
        .subscribe(text => this.paragraph.innerHTML = text);
    }
  }

  private disable(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
      this.subscription = undefined;
      this.paragraph.innerHTML += " (zliczanie wyłączone)";
    }
  }
}