import { Component, signal } from '@angular/core';

interface FavoriteLink {
  id: number;
  name: string;
  url: string;
}

@Component({
  selector: 'app-zadanie6',
  imports: [],
  templateUrl: './zadanie6.html',
  styleUrl: './zadanie6.css',
})
export class Zadanie6 {
  protected readonly links = signal<FavoriteLink[]>([
    { id: 1, name: 'Dokumentacja Angular', url: 'https://angular.dev' },
    { id: 2, name: 'TypeScript Handbook', url: 'https://www.typescriptlang.org' },
    { id: 3, name: 'Dokumentacja RxJS', url: 'https://rxjs.dev' },
    { id: 4, name: 'MDN Web Docs', url: 'https://developer.mozilla.org' },
  ]);

  // Przechowuje ID wyłącznie jednego wybranego linku (początkowo żaden nie jest wybrany)
  protected readonly selectedId = signal<number | null>(null);

  protected selectLink(id: number, event: MouseEvent): void {
    event.preventDefault();
    this.selectedId.set(id);
  }
}
