import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'page-details',
  template: `
    <h2>Szczegóły strony</h2>
    <p>ID strony: {{ id }}</p>
  `
})
export class PageDetailsComponent {
  id: string | null;

  constructor(private route: ActivatedRoute) {
    this.id = this.route.snapshot.paramMap.get('id');
  }
}
