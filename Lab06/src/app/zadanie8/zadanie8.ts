import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie8',
  imports: [],
  templateUrl: './zadanie8.html',
  styleUrl: './zadanie8.css',
})
export class Zadanie8 {
  protected readonly count = signal(0);

  // Jedna wspólna metoda obsługująca wszystkie przyciski
  protected changeCount(step: number): void {
    this.count.update((c) => c + step);
  }
}
