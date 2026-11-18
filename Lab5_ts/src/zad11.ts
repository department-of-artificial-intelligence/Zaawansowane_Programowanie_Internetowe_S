// Zadanie 11 - zliczamy kliknięcia przycisku 1, przyciski 2 i 3 włączają/wyłączają
// wyświetlanie licznika w polu input 1 i input 2.
// Parametr useShareReplay pozwala porównać wersję bez i z operatorem shareReplay.
import { fromEvent, Observable, Subscription } from "rxjs";
import { scan, shareReplay } from "rxjs/operators";

export class Zad11 {
  private countButton: HTMLButtonElement;
  private toggle1Button: HTMLButtonElement;
  private toggle2Button: HTMLButtonElement;
  private input1: HTMLInputElement;
  private input2: HTMLInputElement;
  private count$: Observable<number>;
  private subscription1?: Subscription;
  private subscription2?: Subscription;

  constructor(countButtonId: string, toggle1ButtonId: string, toggle2ButtonId: string,
              input1Id: string, input2Id: string, useShareReplay: boolean) {
    this.countButton = document.querySelector<HTMLButtonElement>(`#${countButtonId}`)!;
    this.toggle1Button = document.querySelector<HTMLButtonElement>(`#${toggle1ButtonId}`)!;
    this.toggle2Button = document.querySelector<HTMLButtonElement>(`#${toggle2ButtonId}`)!;
    this.input1 = document.querySelector<HTMLInputElement>(`#${input1Id}`)!;
    this.input2 = document.querySelector<HTMLInputElement>(`#${input2Id}`)!;

    const clicks$ = fromEvent(this.countButton, "click")
      .pipe(scan((count, _) => count + 1, 0));

    this.count$ = useShareReplay ? clicks$.pipe(shareReplay(1)) : clicks$;
  }

  public start(): void {
    fromEvent(this.toggle1Button, "click")
      .subscribe(() => this.subscription1 = this.toggle(this.subscription1, this.input1));
    fromEvent(this.toggle2Button, "click")
      .subscribe(() => this.subscription2 = this.toggle(this.subscription2, this.input2));
  }

  // jeśli wyświetlanie jest włączone - wyłącza je, w przeciwnym wypadku włącza
  private toggle(subscription: Subscription | undefined,
                 input: HTMLInputElement): Subscription | undefined {
    if (subscription) {
      subscription.unsubscribe();
      return undefined;
    }
    return this.count$.subscribe(count => input.value = String(count));
  }
}