import { Component, computed, signal } from '@angular/core';

@Component({
  selector: 'app-zadanie9',
  imports: [],
  templateUrl: './zadanie9.html',
  styleUrl: './zadanie9.css',
})
export class Zadanie9 {
  // Tablica przechowująca ścieżki do zdjęć z katalogu public/images
  protected readonly images: string[] = [
    'images/photo1.jpg',
    'images/photo2.jpg',
    'images/photo3.jpg',
    'images/photo4.jpg',
  ];

  protected readonly currentIndex = signal(0);
  protected readonly message = signal('');
  protected readonly currentImage = computed(() => this.images[this.currentIndex()]);

  protected prevImage(): void {
    if (this.currentIndex() === 0) {
      this.message.set('To jest pierwsze zdjęcie i nie może być wyświetlone wcześniejsze.');
      return;
    }

    this.message.set('');
    this.currentIndex.update((idx) => idx - 1);
  }

  protected nextImage(): void {
    if (this.currentIndex() === this.images.length - 1) {
      this.message.set('To jest ostatnie zdjęcie i nie może być wyświetlone następne.');
      return;
    }

    this.message.set('');
    this.currentIndex.update((idx) => idx + 1);
  }
}
