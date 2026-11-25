import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie1',
  imports: [],
  templateUrl: './zadanie1.html',
  styleUrl: './zadanie1.css',
})
export class Zadanie1 {
  // Reaktywny stan przechowujący tekst do wyświetlenia w paragrafie
  protected readonly fullName = signal('');

  protected onClick(): void {
    // Zmień poniższą wartość na swoje imię i nazwisko
    this.fullName.set('Bartłomiej Latocha');
  }
}
