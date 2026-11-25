import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  //ZADANIE1
  protected readonly fullName = signal('');

  protected showName() {
    this.fullName.set('Jan Kowalski');
  }

  //ZADANIE2
  protected readonly paragraphClass = signal<string | undefined>(undefined);

  protected setLargeText() {
    this.paragraphClass.set('large-text');
  }

  protected setGreenStrike() {
    this.paragraphClass.set('green-strike');
  }

  //ZADANIE3
  protected readonly paragraphClasses3 = signal({
    'large-text': false,
    'green-strike': false,
  });

  protected setLargeText3() {
    this.paragraphClasses3.update((c) => ({ ...c, 'large-text': true }));
  }

  protected setGreenStrike3() {
    this.paragraphClasses3.update((c) => ({ ...c, 'green-strike': true }));
  }
}
