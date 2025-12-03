import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  templateUrl: 'first.component.html',
  standalone: false,
  styleUrl: 'first.component.css',
})
export class FirstComponent implements OnInit {
  title: string = 'Pierwsza wartość przekazana z komponentu';
  person = {
    firstName: 'Ala',
    lastName: 'Nowak',
  };
  h1Class?: string;

  constructor() {}

  metoda() {
    return 'Wartośc zwrócona z metody';
  }
  ngOnInit(): void {}

  onClick() {
    this.title = 'Po kliknieciu';
    this.h1Class = 'Boo';
  }
}
