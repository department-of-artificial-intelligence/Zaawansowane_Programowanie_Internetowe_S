import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie3',
  imports: [],
  templateUrl: './zadanie3.html',
  styleUrl: './zadanie3.css',
})
export class Zadanie3 {
  protected readonly fullName = signal('');
  // Obiekt pozwala niezależnie aktywować każdą z klas CSS
  protected readonly paragraphClasses = signal({
    'font-20': false,
    'green-strike': false,
  });

  protected showName(): void {
    this.fullName.set('Bartlomiej Latocha');
  }

  protected setFontSize20(): void {
    this.paragraphClasses.update((classes) => ({
      ...classes,
      'font-20': true,
    }));
  }

  protected setGreenStrike(): void {
    this.paragraphClasses.update((classes) => ({
      ...classes,
      'green-strike': true,
    }));
  }
}
