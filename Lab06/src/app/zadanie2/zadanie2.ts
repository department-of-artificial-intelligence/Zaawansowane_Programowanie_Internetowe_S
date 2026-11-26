import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie2',
  imports: [],
  templateUrl: './zadanie2.html',
  styleUrl: './zadanie2.css',
})
export class Zadanie2 {
  protected readonly fullName = signal('');
  // Przechowuje nazwę jednej aktywnej klasy CSS (gwarantuje rozłączność efektów)
  protected readonly paragraphClass = signal<string | undefined>(undefined);

  protected showName(): void {
    this.fullName.set('Bartlomiej Latocha');
  }

  protected setFontSize20(): void {
    this.paragraphClass.set('font-20');
  }

  protected setGreenStrike(): void {
    this.paragraphClass.set('green-strike');
  }
}
