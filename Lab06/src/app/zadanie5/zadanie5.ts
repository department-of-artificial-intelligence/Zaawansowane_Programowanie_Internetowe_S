import { Component, signal } from '@angular/core';

interface FavoriteLink {
  id: number;
  name: string;
  url: string;
  selected: boolean;
}

@Component({
  selector: 'app-zadanie5',
  imports: [],
  templateUrl: './zadanie5.html',
  styleUrl: './zadanie5.css',
})
export class Zadanie5 {
  protected readonly links = signal<FavoriteLink[]>([
    { id: 1, name: 'Dokumentacja Angular', url: 'https://angular.dev', selected: false },
    { id: 2, name: 'TypeScript Handbook', url: 'https://www.typescriptlang.org', selected: false },
    { id: 3, name: 'Dokumentacja RxJS', url: 'https://rxjs.dev', selected: false },
    { id: 4, name: 'MDN Web Docs', url: 'https://developer.mozilla.org', selected: false },
  ]);

  protected toggleLink(id: number, event: MouseEvent): void {
    // Zatrzymanie domyślnego przejścia pod adres URL, aby zmiana tła była widoczna na stronie
    event.preventDefault();

    this.links.update((items) =>
      items.map((item) =>
        item.id === id ? { ...item, selected: !item.selected } : item
      )
    );
  }
}
