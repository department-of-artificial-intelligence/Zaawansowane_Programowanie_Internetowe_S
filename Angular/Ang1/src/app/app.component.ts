import { Component } from '@angular/core';
@Component({
  selector: 'app-root',
  template: `
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">Moja lista Kontaktów - przyk!adowa aplikacja</a>
      </div>
    </nav>
    <div class="conteiner-fluid">
      <div class="row">
        <router-outlet></router-outlet>
        /
      </div>
    </div>
  `,
  styles: [],
})
export class AppComponent {
  title = 'Contacts';
}
