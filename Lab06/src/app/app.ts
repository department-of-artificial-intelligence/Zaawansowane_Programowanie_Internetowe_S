import { Component } from '@angular/core';
import { Zadanie1 } from './zadanie1/zadanie1';
import { Zadanie2 } from './zadanie2/zadanie2';

@Component({
  selector: 'app-root',
  imports: [Zadanie1, Zadanie2],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App { }
