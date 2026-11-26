import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie4',
  imports: [],
  templateUrl: './zadanie4.html',
  styleUrl: './zadanie4.css',
})
export class Zadanie4 {
  protected readonly fullName = signal('');
  protected readonly paragraphClasses = signal({
    'font-20': false,
    'green-strike': false,
  });

  protected showName(): void {
    this.fullName.set('Bartlomiej Latocha');
  }

  protected toggleFontSize20(): void {
    this.paragraphClasses.update((classes) => ({
      ...classes,
      'font-20': !classes['font-20'],
    }));
  }

  protected toggleGreenStrike(): void {
    this.paragraphClasses.update((classes) => ({
      ...classes,
      'green-strike': !classes['green-strike'],
    }));
  }
}
