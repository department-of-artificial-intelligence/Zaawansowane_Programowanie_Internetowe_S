import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie12',
  imports: [],
  templateUrl: './zadanie12.html',
  styleUrl: './zadanie12.css',
})
export class Zadanie12 {
  protected readonly stars: number[] = [1, 2, 3, 4, 5];
  protected readonly rating = signal(0);

  protected rate(value: number): void {
    this.rating.set(value);
  }
}
