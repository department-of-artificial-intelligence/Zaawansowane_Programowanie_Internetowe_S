import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <h1>Zadanie 1 — Routing</h1>

    <button routerLink="/home">Home</button>
    <button routerLink="/about">About</button>
    <button routerLink="/contact">Contact</button>

    <hr>

    <router-outlet></router-outlet>
  `
})
export class AppComponent {}
