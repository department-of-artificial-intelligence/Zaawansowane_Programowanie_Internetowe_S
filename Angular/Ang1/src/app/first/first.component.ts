import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  standalone: false,
  template: `
    <button (click)="onClick()">Kliknij mnie</button>
    <h1 [ngClass]="h1Class">
      {{ title }}
      {{ method() }}
    </h1>
  `,
  styleUrls: ['./first.component.css']
})
export class FirstComponent implements OnInit {
  title: string = "Pierwsza wartosc przekazana z komponentu";
  h1Class: {} | undefined;

  constructor() { }

  ngOnInit(): void {

  }

  method() {
    return "wartość zwrócona z metody";
  }

  onClick() {
    this.title = "Po kliknieciu";
    this.h1Class = { "selected": true, "clicked": false };
  }
}
