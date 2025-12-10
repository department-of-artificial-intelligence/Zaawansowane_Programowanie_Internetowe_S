import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'page-remove',
  imports: [RouterLink],
  template: `
    <h2>Czy chcesz usunąć stronę nr {{ id }}?</h2>
    <button routerLink="/">Tak</button>
    <button routerLink="/">Nie</button>
  `
})
export class PageRemoveComponent {
  id: string | null = null;

  constructor(private route: ActivatedRoute) {
    this.id = this.route.snapshot.paramMap.get('id');
  }
}
