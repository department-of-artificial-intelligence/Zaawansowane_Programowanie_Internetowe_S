import { Component, computed, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie10',
  imports: [],
  templateUrl: './zadanie10.html',
  styleUrl: './zadanie10.css',
})
export class Zadanie10 {
  protected readonly images: string[] = [
    'images/photo1.jpg',
    'images/photo2.jpg',
    'images/photo3.jpg',
    'images/photo4.jpg',
  ];

  protected readonly currentIndex = signal(0);
  protected readonly currentImage = computed(() => this.images[this.currentIndex()]);
  protected readonly isFirst = computed(() => this.currentIndex() === 0);
  protected readonly isLast = computed(() => this.currentIndex() === this.images.length - 1);

  protected prevImage(): void {
    if (!this.isFirst()) {
      this.currentIndex.update((idx) => idx - 1);
    }
  }

  protected nextImage(): void {
    if (!this.isLast()) {
      this.currentIndex.update((idx) => idx + 1);
    }
  }
}
