import { fromEvent, tap } from 'rxjs';

class ClickCounterApp {
  private clickCount: number = 0;
  private button: HTMLButtonElement;
  private paragraph: HTMLParagraphElement;

  constructor(buttonId: string, paragraphId: string) {
    this.button = document.getElementById(buttonId) as HTMLButtonElement;
    this.paragraph = document.getElementById(paragraphId) as HTMLParagraphElement;

    this.init();
  }

  private init(): void {
    fromEvent(this.button, 'click')
      .pipe(
        tap(() => {
          this.clickCount++;
        })
      )
      .subscribe(() => {
        this.updateView();
      });
  }

  private updateView(): void {
    this.paragraph.textContent = `Liczba kliknięć: ${this.clickCount}`;
  }
}

new ClickCounterApp('clickBtn', 'counterParagraph');
