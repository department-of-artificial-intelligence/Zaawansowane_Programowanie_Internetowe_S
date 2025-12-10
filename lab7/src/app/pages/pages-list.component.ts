import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'pages-list',
  imports: [RouterLink],
  template: `
    <h2>Lista stron</h2>
    <ul>
      <li *ngFor="let p of pages">
        {{p.title}}
        <a [routerLink]="['/pages', p.id, 'details']">Szczegóły</a>
        <a [routerLink]="['/pages', p.id, 'remove']">Usuń</a>
      </li>
    </ul>
    <button routerLink="/add">Dodaj stronę</button>
  `
})
export class PagesListComponent {
  pages = [
    { id: 1, title: 'Strona 1' },
    { id: 2, title: 'Strona 2' }
  ];
}
