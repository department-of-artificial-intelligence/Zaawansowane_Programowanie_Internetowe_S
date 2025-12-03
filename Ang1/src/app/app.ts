import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <h1>Welcome to {{ title() }}!</h1>

    <router-outlet />
  `,
  standalone: false,
  styles: []
})
export class App {
  protected readonly title = signal('Ang1');
}
