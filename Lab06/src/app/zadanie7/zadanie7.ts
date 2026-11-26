import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie7',
  imports: [],
  templateUrl: './zadanie7.html',
  styleUrl: './zadanie7.css',
})
export class Zadanie7 {
  protected readonly count = signal(0);

  protected increment(): void {
    this.count.update((c) => c + 1);
  }

  protected decrement(): void {
    this.count.update((c) => c - 1);
  }
}
